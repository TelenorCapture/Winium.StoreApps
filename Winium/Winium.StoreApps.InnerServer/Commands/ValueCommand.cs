namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal class ValueCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        public string KeyString { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);

            if (element is TextBox)
                TrySetText(element as TextBox, this.KeyString);
            else if (element is PasswordBox)
                TrySetText(element as PasswordBox, this.KeyString);
            else
                throw new AutomationException("Element referenced is not a TextBox.", ResponseStatus.UnknownError);

            return this.JsonResponse();
        }

        #endregion

        #region Methods

        private static void TrySetText(TextBox textbox, string text)
        {
            // TODO: why IValueProvider is null in UniApp?
            var peer = new TextBoxAutomationPeer(textbox);
            var valueProvider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            if (valueProvider != null)
            {
                valueProvider.SetValue(text);
            }
            else
            {
                textbox.Text = text;
                textbox.SelectionStart = text.Length;
            }

            // TODO: new parameter - FocusState
            textbox.Focus(FocusState.Pointer);
        }

        private static void TrySetText(PasswordBox pBox, string text)
        {
            // TODO: why IValueProvider is null in UniApp?
            var peer = new PasswordBoxAutomationPeer(pBox);
            var valueProvider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            if (valueProvider != null)
            {
                valueProvider.SetValue(text);
            }
            else
            {
                pBox.Password = text;
            }

            // TODO: new parameter - FocusState
            pBox.Focus(FocusState.Pointer);
        }

        #endregion
    }
}
