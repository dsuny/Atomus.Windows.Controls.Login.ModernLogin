using Atomus.Control;
using Atomus.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atomus.Windows.Controls.Login
{
    /// <summary>
    /// DefaultLogin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModernLogin : UserControl, IAction
    {
        private AtomusControlEventHandler beforeActionEventHandler;
        private AtomusControlEventHandler afterActionEventHandler;

        private bool isPadeIn;
        private Timer timerFade;

        #region Init
        public ModernLogin()
        {
            this.Resources = Application.Current.Windows[0].Resources;

            //this.Style = (System.Windows.Style)this.Resources.MergedDictionaries[0]["UserControlDefaultLogin"];


            InitializeComponent();

            this.timerFade = new Timer
            {
                Interval = 10
            };
            this.timerFade.Elapsed += this.TimerFade_Tick;

            this.DataContext = new ViewModel.ModernLoginViewModel(this);

            this.ControlAction(this, "Opacity.Set", 0D);

            //this.Email.SetValidator("Email", UpdateSourceTrigger.PropertyChanged);
            //this.AccessNumber.SetValidator("AccessNumber", UpdateSourceTrigger.PropertyChanged);
            //this.Language.SetValidator("LanguageSelected", UpdateSourceTrigger.PropertyChanged);
        }


        #endregion

        #region Dictionary
        #endregion

        #region Spread
        #endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            AtomusControlEventArgs atomusControlEventArgs;

            try
            {
                this.beforeActionEventHandler?.Invoke(this, e);

                switch (e.Action)
                {
                    case "Email.Focus":
                        this.Email.Focus();
                        return true;

                    case "AccessNumber.Focus":
                        //this.window.Opacity = 1;
                        //this.window.Show();
                        this.AccessNumber.Focus();
                        return true;

                    case "Form.Size":
                        if (this.Background == null)//이미지를 늦게 불러오는 경우에 다시 반영
                            this.Background = (this.DataContext as ViewModel.ModernLoginViewModel).BackgroundImage;
                        return true;

                    case "Login.Cancel":
                        return true;

                    case "Opacity.Set":
                        return true;

                    case "Opacity.Get":
                        atomusControlEventArgs = new AtomusControlEventArgs("Opacity.Get", null);
                        this.afterActionEventHandler?.Invoke(this, atomusControlEventArgs);

                        return atomusControlEventArgs.Value;

                    case "Login.Ok":
                        return true;

                    case "Login.JoinOrAccessNumberChange":
                        return true;

                    case "Login.LanguageSelected":
                        this.Translator().Restoration(this);
                        this.Translator().Translate(this);
                        return true;

                    default:
                        throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            finally
            {
                if (!new string[] { "Email.Focus", "AccessNumber.Focus", "Opacity.Get", "Login.LanguageSelected" }.Contains(e.Action))
                    this.afterActionEventHandler?.Invoke(this, e);
            }
        }

        #endregion

        #region Event
        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private void DefaultLogin_Loaded(object sender, RoutedEventArgs e)
        {
            ImageSource imageSource;
            try
            {
                if (this.Background != null)
                {
                    imageSource = (this.Background as ImageBrush).ImageSource;
                    this.ControlAction(this, "Form.Size", new System.Windows.Size(imageSource.Width, imageSource.Height));
                }

                this.FadeInStrart();
            }
            catch (Exception exception)
            {
                //this.WindowsMessageBoxShow(Application.Current.Windows[0], exception);
            }
        }

        private void AccessNumber_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModel.ModernLoginViewModel).AccessNumber = (sender as PasswordBox).Password;
        }

        private void MouseLeftButtonDownDragMove(object sender, MouseButtonEventArgs e)
        {
            this.afterActionEventHandler?.Invoke(this, new AtomusControlEventArgs("DragMove", null));
        }

        Window window;
        private void TimerFade_Tick(object sender, ElapsedEventArgs e)
        {
            Timer timer;
            double opacity;

            timer = (Timer)sender;

            try
            {
                if (Config.Client.GetAttribute("Sessionkey") != null)
                {
                    timer?.Stop();

                    this.ControlAction(this, "Opacity.Set", 1D);

                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate
                    {
                        (this.DataContext as ViewModel.ModernLoginViewModel).LoginCommand.Execute(null);
                    }));
                }
                else
                {
                    opacity = (double)this.ControlAction(this, "Opacity.Get", null);

                    if (this.isPadeIn)
                    {
                        if (opacity >= 1.0D)
                        {
                            timer.Enabled = false;

                            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Action(delegate
                            {
                                if ((this.DataContext as ViewModel.ModernLoginViewModel).IsAutoLogin)
                                {
                                    if ((this.DataContext as ViewModel.ModernLoginViewModel).IsAutoLogin && this.WindowsMessageBoxShow(Application.Current.Windows[0], "자동로그인 하시겠습니까?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                        (this.DataContext as ViewModel.ModernLoginViewModel).LoginCommand.Execute(null);
                                }
                            }));



                        }
                        else
                        {
                            this.ControlAction(this, "Opacity.Set", opacity += 0.01D);
                        }
                    }
                    else
                    {
                        if (opacity <= 0.0D)
                            timer?.Stop();
                        else
                            this.ControlAction(this, "Opacity.Set", opacity -= 0.01D);
                    }
                }
            }
            catch (Exception exception)
            {
                timer?.Stop();

                DiagnosticsTool.MyTrace(exception);

                try
                {
                    this.ControlAction(this, "Opacity.Set", 1.0D);
                }
                catch (Exception ex)
                {
                    DiagnosticsTool.MyTrace(ex);

                }
            }
        }
        #endregion

        #region ETC
        private void FadeInStrart()
        {
            this.isPadeIn = true;
            this.timerFade.Enabled = true;
        }
        #endregion

    }
}