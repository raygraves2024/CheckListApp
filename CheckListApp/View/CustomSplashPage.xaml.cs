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

            // Set initial opacity to 0
            if (LogoImage != null) LogoImage.Opacity = 0;
            if (ProjectLabel != null) ProjectLabel.Opacity = 0;
            if (Member1 != null) Member1.Opacity = 0;
            if (Member2 != null) Member2.Opacity = 0;
            if (Member3 != null) Member3.Opacity = 0;
            if (Member4 != null) Member4.Opacity = 0;
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

                    // Animate elements flying out
                    var flyOutTasks = new[]
                    {
                        // Logo flies up
                        Task.WhenAll(
                            LogoImage.TranslateTo(0, -1000, 500, Easing.CubicIn),
                            LogoImage.FadeTo(0, 500)
                        ),
                        // Project label flies right
                        Task.WhenAll(
                            ProjectLabel.TranslateTo(1000, 0, 500, Easing.CubicIn),
                            ProjectLabel.FadeTo(0, 500)
                        ),
                        // Members fly left/right alternately
                        Task.WhenAll(
                            Member1.TranslateTo(-1000, 0, 500, Easing.CubicIn),
                            Member1.FadeTo(0, 500)
                        ),
                        Task.WhenAll(
                            Member2.TranslateTo(1000, 0, 500, Easing.CubicIn),
                            Member2.FadeTo(0, 500)
                        ),
                        Task.WhenAll(
                            Member3.TranslateTo(-1000, 0, 500, Easing.CubicIn),
                            Member3.FadeTo(0, 500)
                        ),
                        Task.WhenAll(
                            Member4.TranslateTo(1000, 0, 500, Easing.CubicIn),
                            Member4.FadeTo(0, 500)
                        )
                    };

                    await Task.WhenAll(flyOutTasks);

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
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Animation error: {ex.Message}");
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