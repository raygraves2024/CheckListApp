using CheckListApp.Services;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace CheckListApp.View
{
    public partial class CustomSplashPage : ContentPage
    {
        public CustomSplashPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Start animations as soon as the page appears
            Task.Run(async () => await AnimateSplashScreen());
        }

        private async Task AnimateSplashScreen()
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // Ensure page is loaded
                    await Task.Delay(100);

                    // Fade in logo and project name
                    await Task.WhenAll(
                        LogoImage.FadeTo(1, 1000),
                        ProjectLabel.FadeTo(1, 1000)
                    );

                    // Animate team members flying in
                    var animationTasks = new[]
                    {
                        AnimateMember(Member1, true),
                        AnimateMember(Member2, false),
                        AnimateMember(Member3, true),
                        AnimateMember(Member4, false)
                    };

                    await Task.WhenAll(animationTasks);

                    // Wait for additional 2 seconds after animations complete
                    await Task.Delay(2000);

                    // Fade out all elements
                    var fadeOutTasks = new[]
                    {
                        LogoImage.FadeTo(0, 500),
                        ProjectLabel.FadeTo(0, 500),
                        Member1.FadeTo(0, 500),
                        Member2.FadeTo(0, 500),
                        Member3.FadeTo(0, 500),
                        Member4.FadeTo(0, 500)
                    };

                    await Task.WhenAll(fadeOutTasks);

                    // Create a white overlay for transition
                    var overlay = new BoxView
                    {
                        Color = Colors.White,
                        Opacity = 0,
                        ZIndex = 999
                    };

                    // Add the overlay to cover the entire page
                    var grid = LogoImage.Parent as Grid;
                    if (grid != null)
                    {
                        grid.Add(overlay);
                        Grid.SetRowSpan(overlay, 3);
                        Grid.SetColumnSpan(overlay, 2);

                        // Fade in the white overlay
                        await overlay.FadeTo(1, 500);
                    }

                    // Set the main page to AppShell and navigate
                    var shell = Handler.MauiContext?.Services.GetService<AppShell>();
                    if (shell != null)
                    {
                        Application.Current.MainPage = shell;

                        // Then navigate based on authentication
                        var authService = Handler.MauiContext?.Services.GetService<IAuthenticationService>();
                        if (authService != null)
                        {
                            if (authService.IsAuthenticated)
                            {
                                await Shell.Current.GoToAsync("//LoginPage");
                            }
                            else
                            {
                                await Shell.Current.GoToAsync("//RegistrationPage");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Authentication service not found");
                            await Shell.Current.GoToAsync("//LoginPage");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("AppShell service not found");
                        // Fallback to direct page creation if service isn't available
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Animation error: {ex.Message}");

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // Fallback navigation in case of animation failure
                    try
                    {
                        var shell = Handler.MauiContext?.Services.GetService<AppShell>();
                        Application.Current.MainPage = shell ?? new AppShell();
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                    catch (Exception navEx)
                    {
                        Debug.WriteLine($"Navigation error: {navEx.Message}");
                    }
                });
            }
        }

        private async Task AnimateMember(Label member, bool fromLeft)
        {
            if (member == null) return;

            try
            {
                // Parse the member number from the StyleId
                var memberNumber = 0;
                if (member.StyleId?.StartsWith("Member") == true)
                {
                    _ = int.TryParse(member.StyleId.Replace("Member", ""), out memberNumber);
                }

                // Wait for a small delay before starting each member's animation
                await Task.Delay(200 * (memberNumber > 0 ? memberNumber : 1));

                // Create combined animation
                await Task.WhenAll(
                    member.TranslateTo(0, 0, 1000, Easing.SpringOut),
                    member.FadeTo(1, 800)
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Member animation error for {member.StyleId}: {ex.Message}");
                // Ensure the member is visible even if animation fails
                member.Opacity = 1;
                member.TranslationX = 0;
            }
        }
    }
}