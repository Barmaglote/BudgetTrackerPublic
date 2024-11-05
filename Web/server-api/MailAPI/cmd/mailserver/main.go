package main

import (
	"barmaglote/mail-api/internal/app/mailserver"
	"flag"
	"log"
	"sync"

	"github.com/BurntSushi/toml"
	"github.com/joho/godotenv"
)

var (
	configPath string
)

func init() {
	flag.StringVar(&configPath, "config-path", "configs/mailserver.toml", "path to config file")
	if err := godotenv.Load(); err != nil {
		log.Print("No .env file found")
	}
}

func main() {
	log.Println("Mail server is starting")
	flag.Parse()

	httpServerExitDone := &sync.WaitGroup{}
	httpServerExitDone.Add(1)

	config := mailserver.NewConfig()
	_, err := toml.DecodeFile(configPath, config)

	if err != nil {
		log.Fatal(err)
	}

	s := mailserver.New(config)
	defer stop(s)

	if err := s.Start(httpServerExitDone); err != nil {
		log.Println("Error during start")
	}
	
	log.Println("Mail server is started")

	httpServerExitDone.Wait()
}

func stop(s *mailserver.MailServer) {
	s.Stop()
	log.Println("Mail server is stopped")
}
