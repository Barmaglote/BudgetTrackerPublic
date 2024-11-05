package mailserver

import (
	"bytes"
	"fmt"
	"html/template"
	"log"
	"net/http"
	"net/smtp"
	"strings"
	"sync"

	"github.com/gin-gonic/gin"
	"github.com/nullseed/logruseq"
	"github.com/sirupsen/logrus"
)

type Msg struct {
	Category     string `json:"Category"`     // Cofirmation, Request and etc. TODO: enum
	Username     string `json:"Username"`     // Title, Username
	Email        string `json:"Email"`        // To
	Confirmation string `json:"Confirmation"` // Confirmation
}

type Mail struct {
	Sender  string
	To      []string
	Subject string
	Body    string
}

type health struct {
	Msg string `json:"Msg"`
}

type templateData struct {
	Snippet *Msg
}

var healthresponse = []health{
	{Msg: "Status is ok"},
}

type MailServer struct {
	config *Config
	logger *logrus.Logger
	router *gin.Engine
	queue  chan *Msg
	sync.Mutex
	tmpls       map[string]*template.Template
	tmplsConfig map[string]*TemplateConfig
	env         *EnvConfig
	instance    *http.Server
}

func New(config *Config) *MailServer {
	router := gin.Default()

	return &MailServer{
		config:      config,
		logger:      logrus.New(),
		router:      router,
		queue:       make(chan *Msg, config.MailBufferSize),
		tmpls:       map[string]*template.Template{},
		tmplsConfig: map[string]*TemplateConfig{},
		env:         NewEnv(),
		instance:    &http.Server{Addr: config.BindAddr, Handler: router},
	}
}

func (s *MailServer) Start(wg *sync.WaitGroup) error {

	if err := s.configureLogger(); err != nil {
		return err
	}

	if err := s.configureTemplates(); err != nil {
		return err
	}

	if err := s.configureRouter(); err != nil {
		return err
	}

	go func() {
		defer wg.Done()
		s.logger.WithField("application", "mailapi").Info("Mail server is starting")
		if err := s.instance.ListenAndServe(); err != http.ErrServerClosed {
			s.logger.WithField("application", "mailapi").Info("Mail server is unable to start")
			s.instance.Close()
		}

		s.logger.WithField("application", "mailapi").Info("Mail server is started")
	}()

	return nil
}

func (s *MailServer) configureLogger() error {
	level, err := logrus.ParseLevel(s.config.LogLevel)
	if err != nil {
		return err
	}

	s.logger.AddHook(logruseq.NewSeqHook(s.env.LogConfig.Addr, logruseq.OptionAPIKey(s.env.LogConfig.APIKey)))
	s.logger.SetReportCaller(true)
	s.logger.SetLevel(level)

	return nil
}

func (s *MailServer) configureTemplates() error {
	for _, item := range s.config.Templates {
		files := make([]string, 0)
		var sb strings.Builder

		for _, file := range item.Files {
			sb.Reset()
			sb.WriteString(s.config.TemplateFolder)
			sb.WriteString(file)
			files = append(files, sb.String())
		}

		ts, err := template.ParseFiles(files...)

		if err != nil {
			s.logger.Error(err.Error())
			s.logger.Error("Unable to read file")
			return nil
		}

		s.tmpls[item.Name] = ts
		s.tmplsConfig[item.Name] = item
	}

	return nil
}

func (s *MailServer) configureRouter() error {
	s.router.GET("/v1/health", getStatus)
	s.router.POST("/v1/send", sendMessage(s))
	return nil
}

func getStatus(c *gin.Context) {
	c.JSON(http.StatusOK, healthresponse)
}

func sendMessage(s *MailServer) gin.HandlerFunc {
	go notify(s.queue, s)

	return func(c *gin.Context) {
		if !isAuthorized(s, c) {
			c.String(401, "Failed")
			return
		}

		var msg Msg
		if c.ShouldBind(&msg) == nil {
			s.queue <- &msg
		}

		c.String(200, "Success")
	}
}

func (s *MailServer) Stop() {
	s.logger.WithField("application", "mailapi").Info("Server has recieved signal to stop")
	if err := s.instance.Close(); err != nil {
		s.logger.WithField("application", "mailapi").Error("Server is unable to close connections")
	}

	notify(s.queue, s)
	close(s.queue)
}

func notify(c chan *Msg, s *MailServer) {

	for msg := range c {
		message := ""
		buf := bytes.NewBufferString(message)
		data := &templateData{Snippet: msg}

		s.Lock()
		tmpl := s.tmpls[msg.Category]
		s.Unlock()

		if tmpl == nil {
			s.logger.Error("Unable to find template")
			return
		}

		err := tmpl.Execute(buf, data)

		if err != nil {
			log.Println(err.Error())
			s.logger.Error("Unable to parse")
		}

		go send(msg, buf.String(), s)
	}
}

func send(messsage *Msg, body string, s *MailServer) {
	s.logger.Debug("Email is add to queue")

	to := []string{
		messsage.Email,
	}

	request := Mail{
		Sender:  s.config.MailSender,
		To:      to,
		Subject: s.tmplsConfig[messsage.Category].Subject,
		Body:    body,
	}

	msg := BuildMessage(request)

	auth := smtp.PlainAuth("Reputation", s.env.Mail.Username, s.env.Mail.Password, s.config.SMTPHost)

	s.logger.Error(auth)

	err := smtp.SendMail(s.config.SMTPAddr, auth, s.config.MailSender, to, []byte(msg))

	if err != nil {
		s.logger.Debug("Email was not sent")
		s.logger.Error(err)
	} else {
		s.logger.Debug("Email sent successfully")
	}
}

func BuildMessage(mail Mail) string {
	msg := "MIME-version: 1.0;\nContent-Type: text/html; charset=\"UTF-8\";\r\n"
	msg += fmt.Sprintf("From: %s\r\n", mail.Sender)
	msg += fmt.Sprintf("To: %s\r\n", strings.Join(mail.To, ";"))
	msg += fmt.Sprintf("Subject: %s\r\n", mail.Subject)
	msg += fmt.Sprintf("\r\n%s\r\n", mail.Body)

	return msg
}

func isAuthorized(s *MailServer, c *gin.Context) bool {
	apikey := c.Request.Header.Get("APIKEY")
	return apikey == s.env.APIKey
}
