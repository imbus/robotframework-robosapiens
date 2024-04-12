using sapfewse;

namespace RoboSAPiens {
    public record StatusbarMessage(string status, string message);

    public class SAPStatusbar {
        enum MessageType {
            Abort,
            Error,
            Information,
            Success,
            Warning,
            Empty
        }
        public string id {get;}
        string message;
        MessageType messageType;

        public SAPStatusbar(GuiStatusbar statusBar) {
            this.id = statusBar.Id;
            this.message = statusBar.Text;
            this.messageType = getMessageType(statusBar.MessageType);
        }

        MessageType getMessageType(string sapMessageType) {
            return sapMessageType switch {
                "A" => MessageType.Abort,
                "E" => MessageType.Error,
                "I" => MessageType.Information,
                "S" => MessageType.Success,
                "W" => MessageType.Warning,
                _ => MessageType.Empty
            };
        }

        public string getMessage() {
            var statusbarMessage = new StatusbarMessage(messageType.ToString(), message);
            return JSON.serialize(statusbarMessage, typeof(StatusbarMessage));
        }
    }
}
