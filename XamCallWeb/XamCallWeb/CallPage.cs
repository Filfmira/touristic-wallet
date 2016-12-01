using System;
using System.IO;
using System.Net;
using Xamarin.Forms;

namespace XamCallWeb {
  public class CallPage : ContentPage {
    Label lab1, lab2;
    Entry entTo, entFrom;

    public CallPage() {
      var labFrom = new Label() {
        Text = "Convert from:",
        WidthRequest = 100
      };
      entFrom = new Entry() {
        HorizontalOptions = LayoutOptions.StartAndExpand,
        Placeholder = "Currency code"
      };
      var stack1 = new StackLayout() {
        Orientation = StackOrientation.Horizontal,
        Children = { labFrom, entFrom }
      };

      var labTo = new Label() {
        Text = "Convert to:",
        WidthRequest = 100
      };
      entTo = new Entry() {
        HorizontalOptions = LayoutOptions.StartAndExpand,
        Placeholder = "Currency code"
      };
      var stack2 = new StackLayout() {
        Orientation = StackOrientation.Horizontal,
        Children = { labTo, entTo }
      };

      var but = new Button() {
        Text = "Call",
        HorizontalOptions = LayoutOptions.Center
      };
      but.Clicked += OnButton_Clicked;

      lab1 = new Label() {
        HorizontalOptions = LayoutOptions.Center,
        Text = "Status: "
      };

      lab2 = new Label() {
        Text = ""
      };

      Content = new StackLayout {
        Children = { stack1, stack2, but, lab1, lab2 }
      };
    }

    private void OnButton_Clicked(object sender, EventArgs e) {
      var uri = string.Format("http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s={0}{1}=X", entFrom.Text, entTo.Text);
      var cb = new AsyncCallback(CallHandler);
      CallWebAsync(uri, lab1, lab2, cb);
    }

    private void CallWebAsync(string uri, Label status, Label response, AsyncCallback cb) {
      var request = HttpWebRequest.Create(uri);
      request.Method = "GET";
      var state = new Tuple<Label, Label, WebRequest>(status, response, request);

      request.BeginGetResponse(cb, state);
    }

    private void CallHandler(IAsyncResult ar) {
      var state = (Tuple<Label, Label, WebRequest>)ar.AsyncState;
      var request = state.Item3;

      using (HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse) {
        Device.BeginInvokeOnMainThread(() => state.Item1.Text = "Status: " + response.StatusCode);
        if (response.StatusCode == HttpStatusCode.OK)
          using (StreamReader reader = new StreamReader(response.GetResponseStream())) {
            var content = reader.ReadToEnd();
            Device.BeginInvokeOnMainThread(() => state.Item2.Text = content);
          }
      }
    }

  }
}
