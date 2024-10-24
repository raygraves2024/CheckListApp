using System.Text.RegularExpressions;

namespace CheckListApp.Services
{
    public class TextValidationBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty ValidationTypeProperty =
            BindableProperty.Create(nameof(ValidationType), typeof(ValidationType), typeof(TextValidationBehavior), ValidationType.None);

        public static readonly BindableProperty IsValidProperty =
            BindableProperty.Create(nameof(IsValid), typeof(bool), typeof(TextValidationBehavior), false);

        public static readonly BindableProperty MinLengthProperty =
            BindableProperty.Create(nameof(MinLength), typeof(int), typeof(TextValidationBehavior), 0);

        public ValidationType ValidationType
        {
            get => (ValidationType)GetValue(ValidationTypeProperty);
            set => SetValue(ValidationTypeProperty, value);
        }

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        public int MinLength
        {
            get => (int)GetValue(MinLengthProperty);
            set => SetValue(MinLengthProperty, value);
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!(sender is Entry entry))
                return;

            IsValid = ValidateText(args.NewTextValue);
        }

        private bool ValidateText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            if (text.Length < MinLength)
                return false;

            switch (ValidationType)
            {
                case ValidationType.Email:
                    return Regex.IsMatch(text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

                case ValidationType.Password:
                    // Requires at least 8 characters, 1 uppercase, 1 lowercase, 1 number
                    return Regex.IsMatch(text, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");

                case ValidationType.Username:
                    // Alphanumeric characters and underscores, 3-20 characters
                    return Regex.IsMatch(text, @"^[a-zA-Z0-9_]{3,20}$");

                case ValidationType.Name:
                    // Letters, spaces, and hyphens only
                    return Regex.IsMatch(text, @"^[a-zA-Z\s-]+$");

                case ValidationType.None:
                    return true;

                default:
                    return true;
            }
        }
    }

    public enum ValidationType
    {
        None,
        Email,
        Password,
        Username,
        Name
    }
}