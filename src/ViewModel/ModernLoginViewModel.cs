using Atomus.ComponentModel.DataAnnotations;
using Atomus.Control;
using Atomus.Control.Dictionary;
using Atomus.Diagnostics;
using Atomus.Security;
using Atomus.Windows.Controls.Login.Controllers;
using Atomus.Windows.Controls.Login.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Atomus.Windows.Controls.Login.ViewModel
{
    public class ModernLoginViewModel : Atomus.MVVM.ViewModel
    {
        #region Declare
        private List<string> languageList;
        private string languageSelected;

        private string title;
        private string email;
        private string accessNumber;
        private string accessNumberTag;
        private bool isEmailSave;
        private bool isAutoLogin;

        private System.Windows.Visibility joinVisibility;

        private Brush backgroundImage;

        private bool isEnabledIsEmailSaveControl;
        private bool isEnabledControl;
        #endregion

        #region Property
        public ICore Core { get; set; }

        public string Title
        {
            get
            {
                return Factory.FactoryConfig.GetAttribute("Atomus", "ServiceName"); ;
            }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                }
            }
        }
        public List<string> LanguageList
        {
            get
            {
                return this.languageList;
            }
            set
            {
                if (this.languageList != value)
                {
                    this.languageList = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        [Required]
        [Display(Name = "언어")]
        public string LanguageSelected
        {
            get
            {
                if (this.languageSelected == null)
                {
                    if (!Properties.Settings.Default.Language.Equals(""))
                        this.languageSelected = Properties.Settings.Default.Language;
                    else
                        this.languageSelected = Properties.Settings.Default.Language = "ko-KR";

                    this.Translator().TargetCultureName = this.languageSelected;

                    if (!this.languageSelected.IsNullOrEmpty())
                        (this.Core as IAction).ControlAction(this, new AtomusControlArgs("Email.Focus", null));
                }

                return this.languageSelected;
            }
            set
            {
                if (this.languageSelected != value)
                {
                    this.languageSelected = value;
                    this.NotifyPropertyChanged();

                    this.Translator().TargetCultureName = this.languageSelected;

                    (this.Core as IAction).ControlAction(this, "Login.LanguageSelected");

                    Properties.Settings.Default.Language = this.languageSelected;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private bool isFirstEmail = true;
        //[Required(ErrorMessage = "You must enter a Email.")]
        [Required]
        [EmailAddress]
        [Display(Name ="사용자")]
        public string Email
        {
            get
            {
                if (this.isFirstEmail)
                {
                    this.email = Properties.Settings.Default.Email;

                    if (!this.email.IsNullOrEmpty())
                        (this.Core as IAction).ControlAction(this, new AtomusControlArgs("AccessNumber.Focus", null));

                    this.isFirstEmail = false;
                }

                return this.email;
            }
            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    this.NotifyPropertyChanged();
                    if (Config.Client.GetAttribute("Sessionkey") == null)
                        this.NotifyPropertyChanged("AccessNumber");

                    if (this.isEmailSave)
                        Properties.Settings.Default.Email = this.email;
                    else
                        Properties.Settings.Default.Email = "";

                    //Config.Client.SetAttribute("Account.EMAIL", this.email);

                    Properties.Settings.Default.Save();
                }
            }
        }
        private bool isFirstAccessNumber = true;
        [Required]
        [Display(Name = "비밀번호")]
        public string AccessNumber
        {
            get
            {
                if (this.isFirstAccessNumber && this.IsAutoLogin)
                    this.isFirstAccessNumber = false;

                return this.accessNumber;
            }
            set
            {
                if (this.accessNumber != value)
                {
                    this.accessNumber = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("Email");

                    if (this.isAutoLogin)
                        Properties.Settings.Default.Password = this.accessNumber;
                    else
                        Properties.Settings.Default.Password = "";

                    Properties.Settings.Default.Save();
                }
            }
        }
        public string AccessNumberTag
        {
            get
            {
                return this.accessNumberTag;
            }
            set
            {
                if (this.accessNumberTag != value)
                {
                    this.accessNumberTag = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        private bool isFirstIsEmailSave = true;
        public bool IsEmailSave
        {
            get
            {
                if (this.isFirstIsEmailSave)
                {
                    this.isEmailSave = Properties.Settings.Default.IsEmilSave;
                    this.isFirstIsEmailSave = false;
                }

                return this.isEmailSave;
            }
            set
            {
                if (this.isEmailSave != value)
                {
                    this.isEmailSave = value;
                    this.NotifyPropertyChanged();

                    if (this.isEmailSave)
                        Properties.Settings.Default.Email = this.email;
                    else
                    {
                        Properties.Settings.Default.Email = "";
                    }

                    Properties.Settings.Default.IsEmilSave = this.isEmailSave;
                    Properties.Settings.Default.Save();
                }
            }
        }
        private bool isFirstIsAutoLogin = true;
        public bool IsAutoLogin
        {
            get
            {
                if (this.isFirstIsAutoLogin)
                {
                    this.isAutoLogin = Properties.Settings.Default.AutoLogin;
                    this.isFirstIsAutoLogin = false;

                    if (this.isAutoLogin)
                    {
                        this.accessNumber = Properties.Settings.Default.Password;
                        this.accessNumberTag = Properties.Settings.Default.Password;
                    }
                }

                return this.isAutoLogin;
            }
            set
            {
                if (this.isAutoLogin != value)
                {
                    this.isAutoLogin = value;
                    this.NotifyPropertyChanged();

                    if (this.isAutoLogin)
                    {
                        this.IsEmailSave = true;
                        this.IsEnabledIsEmailSaveControl = false;
                        Properties.Settings.Default.Password = this.accessNumber;
                    }
                    else
                    {
                        this.IsEnabledIsEmailSaveControl = true;
                        Properties.Settings.Default.Password = "";
                    }

                    Properties.Settings.Default.AutoLogin = this.isAutoLogin;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public System.Windows.Visibility JoinVisibility
        {
            get
            {
                return this.joinVisibility;
            }
            set
            {
                if (this.joinVisibility != value)
                {
                    this.joinVisibility = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public Brush BackgroundImage
        {
            get
            {
                return this.backgroundImage;
            }
            set
            {
                if (this.backgroundImage != value)
                {
                    this.backgroundImage = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsEnabledControl
        {
            get
            {
                return this.isEnabledControl;
            }
            set
            {
                if (this.isEnabledControl != value)
                {
                    this.isEnabledControl = value;
                    this.NotifyPropertyChanged();
                }
            }
        }
        public bool IsEnabledIsEmailSaveControl
        {
            get
            {
                if (this.IsAutoLogin)
                    this.isEnabledIsEmailSaveControl = false;

                if (!this.isEnabledControl)
                    this.isEnabledIsEmailSaveControl = false;

                return this.isEnabledIsEmailSaveControl;
            }
            set
            {
                if (this.isEnabledIsEmailSaveControl != value)
                {
                    if (this.IsAutoLogin)
                        this.isEnabledIsEmailSaveControl = false;
                    else
                        this.isEnabledIsEmailSaveControl = value;

                    this.NotifyPropertyChanged();
                }
            }
        }

        public ICommand EmailEnterCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ICommand JoinOrAccessNumberChangeCommand { get; set; }

        #endregion

        #region INIT
        public ModernLoginViewModel()
        {
            this.email = "";
            this.accessNumber = "";
            this.accessNumberTag = "";
            this.isEnabledControl = true;

            this.EmailEnterCommand = new MVVM.DelegateCommand(() => { (this.Core as IAction).ControlAction(this, new AtomusControlArgs("AccessNumber.Focus", null)); }
                                                            , () => { return this.isEnabledControl; });

            this.LoginCommand = new MVVM.DelegateCommand(() => { this.LoginProcess(); }
                                                            , () => { return this.isEnabledControl; });

            this.ExitCommand = new MVVM.DelegateCommand(() => { this.ExitProcess(); }
                                                            , () => { return this.isEnabledControl; });

            this.JoinOrAccessNumberChangeCommand = new MVVM.DelegateCommand(() => { this.JoinOrAccessNumberChangeProcess(); }
                                                            , () => { return this.isEnabledControl; });
        }
        public ModernLoginViewModel(ICore core) : this()
        {
            this.Core = core;

            try
            {
                if (this.Core.GetAttribute("EnableJoin") != null && this.Core.GetAttribute("EnableJoin").Equals("Y"))
                    this.joinVisibility = System.Windows.Visibility.Visible;
                else
                    this.joinVisibility = System.Windows.Visibility.Hidden;

                this.GetBackgroundImage();
                this.SetLanguageList();
                this.ReadSSO();

                this.isEnabledIsEmailSaveControl = true;
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
                //this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        #endregion

        #region IO
        private void LoginProcess()
        {
            try
            {
                this.IsEnabledControl = false;
                (this.LoginCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();


                this.ValidateResult = true;

                this.NotifyPropertyChanged("LanguageSelected");
                this.NotifyPropertyChanged("Email");

                if (Config.Client.GetAttribute("Sessionkey") != null)
                    this.Email = Config.Client.GetAttribute("Sessionkey").ToString();
                else
                    this.NotifyPropertyChanged("AccessNumber");

                if (!this.ValidateResult)
                    return;

                if (this.Login(this.email, this.accessNumber, this.accessNumberTag, this.isAutoLogin))
                {
                    //this.EMAIL.TextChanged -= this.EMAIL_TextChanged;
                    (this.Core as IAction).ControlAction(this, "Login.Ok");
                    //this.Language_SelectedIndexChanged(this.Language, null);

                    if (!this.Translator().SourceCultureName.Equals(this.Translator().TargetCultureName))
                    {
                        //this.Translator().Restoration(this.Controls);
                        //this.Translator().Translate(this.Controls);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
            finally
            {
                this.IsEnabledControl = true;
                (this.LoginCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();
            }
        }
        private bool Login(string EMAIL, string ACCESS_NUMBER, string AUTO_LOGIN_ACCESS_NUMBER, bool AUTO_LOGIN)
        {
            Service.IResponse result;
            ISecureHashAlgorithm secureHashAlgorithm;
            string LoginType;
            string LDAPUrl;
            string Domain;

            try
            {
                secureHashAlgorithm = ((ISecureHashAlgorithm)this.Core.CreateInstance("SecureHashAlgorithm"));

                if (Config.Client.GetAttribute("Sessionkey") != null)
                {
                    AUTO_LOGIN = true;
                    AUTO_LOGIN_ACCESS_NUMBER = secureHashAlgorithm.ComputeHashToBase64String(this.Core.GetAttribute("Guid"));
                }
                else
                {
                    LoginType = this.Core.GetAttribute("LoginType");
                    LDAPUrl = this.Core.GetAttribute("LDAPUrl");
                    Domain = this.Core.GetAttribute("Domain");

                    if (!LoginType.IsNullOrWhiteSpace() && LoginType == "LDAP" && !LDAPUrl.IsNullOrWhiteSpace())
                    {
                        try
                        {
                            this.LDAPQuery(LDAPUrl, Domain, EMAIL, ACCESS_NUMBER);

                            AUTO_LOGIN = true;
                            AUTO_LOGIN_ACCESS_NUMBER = secureHashAlgorithm.ComputeHashToBase64String(this.Core.GetAttribute("Guid"));
                        }
                        catch (Exception ex)
                        {
                            this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
                            return false;
                        }
                    }
                }

                //자동 로그인 이고 저장된 패스워드가 없으면 현재 입력한 패스워드를 암호화
                //저장된 패스워드가 있으면 패스워드는 암호화되어 있기 때문에 그냥 패스
                if (AUTO_LOGIN && (AUTO_LOGIN_ACCESS_NUMBER == null || AUTO_LOGIN_ACCESS_NUMBER == ""))
                    AUTO_LOGIN_ACCESS_NUMBER = secureHashAlgorithm.ComputeHashToBase64String(ACCESS_NUMBER);

                result = this.Core.Search(new DefaultLoginSearchModel()
                {
                    EMAIL = EMAIL,
                    ACCESS_NUMBER = (AUTO_LOGIN) ? AUTO_LOGIN_ACCESS_NUMBER : secureHashAlgorithm.ComputeHashToBase64String(ACCESS_NUMBER)//자동 로그인이 아니면 패스워드 암호화해서 전송
                });

                if (result.Status == Service.Status.OK)
                {
                    if (result.DataSet != null && result.DataSet.Tables.Count >= 1)
                        foreach (DataTable _DataTable in result.DataSet.Tables)
                            for (int i = 1; i < _DataTable.Columns.Count; i++)
                                foreach (DataRow _DataRow in _DataTable.Rows)
                                    Config.Client.SetAttribute(string.Format("{0}.{1}", _DataRow[0].ToString(), _DataTable.Columns[i].ColumnName), _DataRow[i]);

                    if (AUTO_LOGIN)
                    {
                        Properties.Settings.Default.Password = AUTO_LOGIN_ACCESS_NUMBER;
                        Properties.Settings.Default.Save();
                    }

                    return true;
                }
                else
                {
                    this.WindowsMessageBoxShow(Application.Current.Windows[0], result.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }

            return false;
        }
        private void LDAPQuery(string LDAPUrl, string domain, string userID, string password)
        {
            System.Net.Mail.MailAddress mailAddres;
            System.DirectoryServices.DirectoryEntry directoryEntry;
            System.DirectoryServices.DirectorySearcher directorySearcher;
            System.DirectoryServices.SearchResult searchResult;

            if (this.IsMailAddress(userID, out mailAddres) && mailAddres != null)
            {
                userID = mailAddres.User;
                domain = mailAddres.Host;
            }

            if (userID.Contains("\\"))
            {
                domain = userID.Substring(0, userID.IndexOf('\\'));
                userID = userID.Substring(userID.IndexOf('\\') + 1);
            }

            directoryEntry = new System.DirectoryServices.DirectoryEntry(LDAPUrl, string.Format("{0}\\{1}", domain, userID), password);
            directorySearcher = new System.DirectoryServices.DirectorySearcher(directoryEntry);
            directorySearcher.ClientTimeout = new TimeSpan(3000);

            directorySearcher.Filter = string.Format("(SAMAccountName={0})", userID);
            searchResult = directorySearcher.FindOne();

            if (searchResult == null)
                throw new Exception("Not found Valid User");
            else
                Config.Client.SetAttribute("System.DirectoryServices.SearchResult", searchResult);
        }

        private void ReadSSO()
        {
            string SSOUser;
            string TimeKey;


            SSOUser = Config.Client.GetAttribute("UriParameter.SSOUser")?.ToString();
            TimeKey = Config.Client.GetAttribute("UriParameter.TimeKey")?.ToString();

            if (SSOUser != null && SSOUser.Length > 0 && TimeKey != null && TimeKey.Length > 0)
                try
                {
                    DiagnosticsTool.MyTrace(new Exception(SSOUser));
                    Config.Client.SetAttribute("Sessionkey", Decrypt(HttpUtility.UrlDecode(SSOUser), TimeKey));
                }
                catch (Exception ex)
                {
                    DiagnosticsTool.MyTrace(ex);
                }

            //if (Config.Client.GetAttribute("Sessionkey") != null)
            //    this.Bnt_Login_Click(this.Bnt_Login, null);
        }

        private void ExitProcess()
        {
            try
            {
                this.IsEnabledControl = false;
                (this.ExitCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();

                (this.Core as IAction).ControlAction(this, new AtomusControlArgs("Login.Cancel", null));
            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
            finally
            {
                this.IsEnabledControl = true;
                (this.ExitCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();
            }
        }

        private void JoinOrAccessNumberChangeProcess()
        {
            try
            {
                this.IsEnabledControl = false;
                (this.JoinOrAccessNumberChangeCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();

                (this.Core as IAction).ControlAction(this, new AtomusControlArgs("Login.JoinOrAccessNumberChange", null));
            }
            catch (Exception ex)
            {
                this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
            finally
            {
                this.IsEnabledControl = true;
                (this.JoinOrAccessNumberChangeCommand as Atomus.MVVM.DelegateCommand).RaiseCanExecuteChanged();
            }
        }

        private async void GetBackgroundImage()
        {
            ImageSource image;

            try
            {
                image = await this.Core.GetAttributeMediaWebImage("BackgroundImage");

                if (image != null)
                {
                    this.backgroundImage = new ImageBrush(image);

                    if (this.backgroundImage != null)
                        (this.Core as IAction).ControlAction(this, new AtomusControlArgs("Form.Size", new System.Windows.Size(image.Width, image.Height)));
                }
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
            }
        }
        #endregion

        #region ETC
        private void SetLanguageList()
        {
            string[] tmps;

            try
            {
                this.languageList = new List<string>();

                var cultureNames = from Cultures in CultureInfo.GetCultures(CultureTypes.AllCultures)
                                   where Cultures.Name.Contains("-")
                                   orderby Cultures.Name
                                   select Cultures.Name;

                if (this.Core.GetAttribute("LanguageList") != null && this.Core.GetAttribute("LanguageList") != "")
                {
                    tmps = this.Core.GetAttribute("LanguageList").Split(',');

                    foreach (string name in tmps)
                        this.languageList.Add(name);
                }
                else
                    foreach (string name in cultureNames)
                        this.languageList.Add(name);

                this.languageList.Add("");
                //if (Properties.Settings.Default.Language.Equals(""))
                //    this.languageSelected = CultureInfo.CurrentCulture.Name;
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
                //this.WindowsMessageBoxShow(Application.Current.Windows[0], ex);
            }
        }

        private string Decrypt(string cipher, string type)
        {
            string EncryptionKey;
            byte[] cipherBytes;

            EncryptionKey = string.Format(this.Core.GetAttribute("DecryptKey"), type);

            cipherBytes = Convert.FromBase64String(cipher);

            using (System.Security.Cryptography.Rijndael encryptor = System.Security.Cryptography.Rijndael.Create())
            {
                System.Security.Cryptography.Rfc2898DeriveBytes pdb = new System.Security.Cryptography.Rfc2898DeriveBytes(EncryptionKey, Convert.FromBase64String(this.Core.GetAttribute("DecryptSalt")));

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, encryptor.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }

                    cipher = System.Text.Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipher;
        }

        private bool IsMailAddress(string mailAddressString, out System.Net.Mail.MailAddress mailAddress)
        {
            try
            {
                mailAddress = new System.Net.Mail.MailAddress(mailAddressString);
                return true;
            }
            catch
            {
                mailAddress = null;
                return false;
            }
        }
        #endregion
    }
}