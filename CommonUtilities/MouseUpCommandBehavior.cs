using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CommonUtilities
{
    public class MouseUpCommandBehavior
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
        "Command", typeof(ICommand), typeof(MouseUpCommandBehavior), new PropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter", typeof(object), typeof(MouseUpCommandBehavior), new PropertyMetadata(null));

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        private static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is FrameworkElement element))
            {
                return;
            }

            if (e.OldValue != null)
            {
                element.RemoveHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(MouseUpHandler));
            }

            if (e.NewValue != null)
            {
                element.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(MouseUpHandler));
            }
        }

        private static void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ICommand command = GetCommand(element);
                object parameter = GetCommandParameter(element);

                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}
