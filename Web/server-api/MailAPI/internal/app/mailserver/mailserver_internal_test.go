package mailserver

import (
	"log"
	"net/http/httptest"
	"testing"

	"github.com/gin-gonic/gin"
)

func TestMailServer_GetStatus(t *testing.T) {
	//s := New(NewConfig())
	rec := httptest.NewRecorder()
	c, _ := gin.CreateTestContext(rec)
	log.Println("TEST")
	getStatus(c)
	log.Println(rec.Body.String())
	//assert.Equal(t, 4, 5)
	//testify.assert.Equal(t, "a", rec.Body.String())
}
