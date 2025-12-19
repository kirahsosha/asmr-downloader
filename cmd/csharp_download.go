package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
)

// callCSharpDownload posts a download request to local C# backend (/api/download).
// Returns ok=true if backend accepted the request (HTTP 200 or 202).
func callCSharpDownload(mediaId string) (bool, error) {
	url := "http://127.0.0.1:9999/api/download"
	payload := map[string]string{"mediaId": mediaId}
	b, _ := json.Marshal(payload)
	resp, err := http.Post(url, "application/json", bytes.NewReader(b))
	if err != nil {
		return false, err
	}
	defer resp.Body.Close()
	_, _ = ioutil.ReadAll(resp.Body)
	if resp.StatusCode == 200 || resp.StatusCode == 202 {
		return true, nil
	}
	return false, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
}
