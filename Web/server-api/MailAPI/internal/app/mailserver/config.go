package mailserver

type TemplateConfig struct {
	Name    string   `toml:"name"`
	Subject string   `toml:"subject"`
	Files   []string `toml:"files"`
}

type Config struct {
	BindAddr       string            `toml:"bind_addr"`
	LogLevel       string            `toml:"log_level"`
	MailBufferSize int               `toml:"mail_buffer_size"`
	TemplateFolder string            `toml:"template_folder"`
	Templates      []*TemplateConfig `toml:"templates"`
	SMTPHost       string            `toml:"smtp_host"`
	SMTPAddr       string            `toml:"smtp_addr"`
	MailSender     string            `toml:"mail_sender"`
}

func NewConfig() *Config {
	return &Config{
		BindAddr:       ":7673",
		LogLevel:       "debug",
		MailBufferSize: 100,
		Templates:      make([]*TemplateConfig, 0, 10),
		SMTPHost:       "",
		SMTPAddr:       "",
		MailSender:     "",
	}
}
