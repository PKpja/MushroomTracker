using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MushroomTracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        private Mushroom mushroom;

        public DetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string json = e.Parameter.ToString();
             mushroom = JsonHandler.deserializeMushroom(json);

             comboBoxKind.SelectedIndex = mushroom.kind;
             comboBoxDensity.SelectedIndex = mushroom.density;

             if (mushroom.objectId == null) {
                 btnDelete.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
             }
           // txtType.Text = mushroom.getUri().ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mushroom.setKindFromInt(comboBoxKind.SelectedIndex);
            mushroom.setDensityFromInt(comboBoxDensity.SelectedIndex);

            startMushroomRequestAsync();
        }

        private async void startMushroomRequestAsync()
        {
            ParseObject parseObject;
            if (mushroom.objectId == null)
            {
                parseObject = new ParseObject("Mushroom");
            }
            else {
                ParseQuery<ParseObject> query = ParseObject.GetQuery("Mushroom");
                parseObject = await query.GetAsync(mushroom.objectId);
            }
                        
            parseObject["Latitude"] = mushroom.coordinate.Latitude;
            parseObject["Longitude"] = mushroom.coordinate.Longitude;
            parseObject["density"] = mushroom.density;
            parseObject["kind"] = mushroom.kind;
            await parseObject.SaveAsync();
            mushroom.objectId = parseObject.ObjectId;
            App.navigationObject = mushroom;
            Frame.GoBack();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            removeAsync();
        }

        private async void removeAsync()
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Mushroom");
            ParseObject parseObject = await query.GetAsync(mushroom.objectId);
            parseObject.DeleteAsync();
            App.navigationObject = mushroom.objectId;
            Frame.GoBack();
        }

       
    }
}
