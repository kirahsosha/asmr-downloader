//go:build ignore
// +build ignore

package cmd

import (
	"fmt"
	"log"

	"github.com/spf13/cobra"
)

var listenPort int

// This listen command is deprecated — use the new C# backend `backend/asmroner` which provides `/api/list` and the Web UI.
var listenCmd = &cobra.Command{
	Use:   "listen",
	Short: "(deprecated) Start web UI — use C# backend instead",
	Run: func(cmd *cobra.Command, args []string) {
		log.Println("[DEPRECATED] The Go 'listen' command has been replaced by the C# backend. Run 'backend/asmroner/asmroner.exe listen -p <port>' instead.")
		fmt.Println("C# backend listen provides the Web UI and /api/list endpoint.")
	},
}

func init() {
	rootCmd.AddCommand(listenCmd)
	listenCmd.Flags().IntVarP(&listenPort, "port", "p", 9999, "服务器端口")
}
