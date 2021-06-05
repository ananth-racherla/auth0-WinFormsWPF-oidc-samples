using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Forms;
using Auth0.OidcClient;
using AuthLib;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace WindowsFormsSample
{
    public partial class Form1 : Form
    {
        private AuthHelper authHelper;

        public Form1()
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = ConfigurationManager.AppSettings["Auth0:Domain"],
                ClientId = ConfigurationManager.AppSettings["Auth0:ClientId"],
                Scope = "openid offline_access"
            });

            authHelper = new AuthHelper(client);
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {


            var extraParameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(connectionNameComboBox.Text))
                extraParameters.Add("connection", connectionNameComboBox.Text);

            if (!string.IsNullOrEmpty(audienceTextBox.Text))
                extraParameters.Add("audience", audienceTextBox.Text);

            var success = await authHelper.LoginAsync(extraParameters: extraParameters);

            if(success)
                DisplayResult();
        }

        private void DisplayResult()
        {

            loginButton.Visible = false;
            logoutButton.Visible = true;

            // Display result
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Tokens");
            sb.AppendLine("------");
            sb.AppendLine($"id_token: {authHelper.GetIDToken()}");
            sb.AppendLine($"access_token: {authHelper.GetAccessToken()}");
            sb.AppendLine($"refresh_token: {authHelper.GetRefreshToken()}");
            sb.AppendLine();

            resultTextBox.Text = sb.ToString();
        }

        private async void LogoutButton_Click(object sender, EventArgs e)
        {
             await authHelper.LogoutAsync();


            logoutButton.Visible = false;
            loginButton.Visible = true;

            resultTextBox.Text = "";
            audienceTextBox.Text = "";
            connectionNameComboBox.Text = "";
        }
    }
}
