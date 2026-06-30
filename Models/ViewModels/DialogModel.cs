using System;

namespace HMS.Models.ViewModels
{
    /// <summary>
    /// Model for the reusable enterprise dialog.
    /// </summary>
    public class DialogModel
    {
        /// <summary>
        /// Dialog title text.
        /// </summary>
        public string Title { get; set; } = "Warning";

        /// <summary>
        /// Main message body.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Text for the confirm button.
        /// </summary>
        public string ConfirmButtonText { get; set; } = "Proceed";

        /// <summary>
        /// Text for the cancel button (optional).
        /// </summary>
        public string CancelButtonText { get; set; } = "Cancel";

        /// <summary>
        /// Razor page handler name that will receive the POST when the user confirms.
        /// </summary>
        public string Handler { get; set; } = string.Empty;

        /// <summary>
        /// Arbitrary identifier for the dialog (e.g., room id or service id).
        /// </summary>
        public string DialogId { get; set; } = string.Empty;

        /// <summary>
        /// Determines whether to show a cancel button.
        /// </summary>
        public bool ShowCancel { get; set; } = true;
    }
}
