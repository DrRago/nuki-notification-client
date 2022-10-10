using Nuki_Opener_Notifier.utils;

namespace Nuki_Opener_Notifier.services
{
    internal class DataService
    {
        public static int GetAuthorizationToken()
        {
            int token;
            bool isValidToken;
            do
            {
                string userInput = UserInterfaceComponents.PromptForNumber("Please enter your authorization token", "Enter Auth Token");
                bool isNumeric = int.TryParse(userInput, out token);
                isValidToken = isNumeric && token.ToString().Length == 6;
                if (!isValidToken)
                {
                    MessageBox.Show("Please enter a six-digit number.", "Auth token validation error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } while (!isValidToken);
            return token;
        }
    }
}
