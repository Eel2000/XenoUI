namespace XenoUI.Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create an instance of the custom XenoView and set it as the content view for the activity, allowing it to be displayed on the screen.
            var xenoView = new Xeno.XenoView(this);

            // Set the content view to the XenoView, which will render the UI components defined in the Xeno framework.
            // This allows the activity to display the custom UI created using Xeno.
            SetContentView(xenoView);

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.activity_main);
        }
    }
}