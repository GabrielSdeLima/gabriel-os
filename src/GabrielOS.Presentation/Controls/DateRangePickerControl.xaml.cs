using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GabrielOS.Presentation.Controls;

public partial class DateRangePickerControl : UserControl
{
    public static readonly DependencyProperty StartDateProperty =
        DependencyProperty.Register(nameof(StartDate), typeof(DateTime), typeof(DateRangePickerControl),
            new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, _) => ((DateRangePickerControl)d).UpdateDisplayText()));

    public static readonly DependencyProperty EndDateProperty =
        DependencyProperty.Register(nameof(EndDate), typeof(DateTime), typeof(DateRangePickerControl),
            new FrameworkPropertyMetadata(DateTime.Today.AddDays(30), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, _) => ((DateRangePickerControl)d).UpdateDisplayText()));

    public DateTime StartDate
    {
        get => (DateTime)GetValue(StartDateProperty);
        set => SetValue(StartDateProperty, value);
    }

    public DateTime EndDate
    {
        get => (DateTime)GetValue(EndDateProperty);
        set => SetValue(EndDateProperty, value);
    }

    private bool _awaitingEnd;
    private bool _suppressChanges;

    public DateRangePickerControl()
    {
        InitializeComponent();
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (DisplayText is null) return;
        DisplayText.Text = $"{StartDate:dd MMM yyyy}  →  {EndDate:dd MMM yyyy}";
    }

    private void MainBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (CalendarPopup.IsOpen)
        {
            CalendarPopup.IsOpen = false;
            return;
        }

        // Defer opening by one dispatcher frame so the current WM_LBUTTONDOWN message
        // finishes processing before the Popup's StaysOpen=False hook registers.
        // Without this, the hook fires for the same click that opened the popup and
        // immediately closes it.
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
        {
            _awaitingEnd = false;
            _suppressChanges = true;
            try
            {
                TheCalendar.SelectedDates.Clear();
                TheCalendar.SelectedDates.AddRange(StartDate.Date, EndDate.Date);
                TheCalendar.DisplayDate = StartDate.Date;
            }
            finally
            {
                _suppressChanges = false;
            }

            CalendarPopup.IsOpen = true;
        }));
    }

    private void TheCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressChanges) return;
        var cal = (Calendar)sender;
        if (cal.SelectedDate is not DateTime picked) return;

        if (!_awaitingEnd)
        {
            // First click: anchor the start date
            StartDate = picked.Date;
            EndDate = picked.Date;
            _awaitingEnd = true;
            // Calendar already shows this single date selected
        }
        else
        {
            // Second click: complete the range and close
            _awaitingEnd = false;

            var start = picked.Date <= StartDate.Date ? picked.Date : StartDate.Date;
            var end   = picked.Date <= StartDate.Date ? StartDate.Date : picked.Date;

            StartDate = start;
            EndDate   = end;

            _suppressChanges = true;
            try
            {
                cal.SelectedDates.Clear();
                cal.SelectedDates.AddRange(start, end);
            }
            finally
            {
                _suppressChanges = false;
            }

            CalendarPopup.IsOpen = false;
        }
    }
}
