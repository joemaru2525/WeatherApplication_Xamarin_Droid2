using System;
using HtmlAgilityPack;

using Android.App;
using Android.Widget;
using Android.OS;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Runtime;
using Android.Views;
using Java.Net;
using Android.Graphics;
using Java.IO;
using Android.Graphics.Drawables;
using Android.Util;
using System.Net;
using System.IO;



namespace YahooWeather_Android
{
    [Activity(Label = "Yahoo!お天気情報", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        public object ImageService { get; private set; }
        public object ImageSource { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);
            button.Click += delegate { button.Text = $"{count++} clicks!"; };


            //(0) Get UI controls from the loaded layout
            var labelTitle = FindViewById<TextView>(Resource.Id.textView1);
            var labelAnnounce = FindViewById<TextView>(Resource.Id.textView2);
            var labelDate = FindViewById<TextView>(Resource.Id.textView3);
            var labelWeather = FindViewById<TextView>(Resource.Id.textView4);
            var labelTempHigh = FindViewById<TextView>(Resource.Id.textView5);
            var labelTempLow = FindViewById<TextView>(Resource.Id.textView6);

            var labelPrecip01 = FindViewById<TextView>(Resource.Id.textView13);
            var labelPrecip02 = FindViewById<TextView>(Resource.Id.textView14);
            var labelPrecip03 = FindViewById<TextView>(Resource.Id.textView15);
            var labelPrecip04 = FindViewById<TextView>(Resource.Id.textView16);

            ImageView imageWeather = FindViewById<ImageView>(Resource.Id.imageView1);


            //(1)指定したサイトのHTMLをストリームで取得する
            const string url = "https://weather.yahoo.co.jp/weather/jp/14/4610.html";
            var urlstring = string.Format(url);

            //(2)指定したサイトのHTMLをストリームで取得する
            var doc = new HtmlAgilityPack.HtmlDocument();
            using (var client = new System.Net.WebClient())
            {
                var html = client.DownloadString(new Uri(urlstring));

                // HtmlAgilityPack.HtmlDocumentオブジェクトにHTMLを読み込ませる
                doc.LoadHtml(html);
            }

            //(3)XPathを使って情報を抽出
            //タイトル
            HtmlNodeCollection node0 =
            doc.DocumentNode.SelectNodes("//title");
            labelTitle.Text = node0[0].InnerText;


            //Anounce Date & Time（発表日時）
            HtmlNodeCollection node1 =
            doc.DocumentNode.SelectNodes("//div[@class='yjw_title_h2 yjw_clr']//p[@class='yjSt yjw_note_h2']");
            labelAnnounce.Text = node1[0].InnerText;

            //WeatherDate（対象日）
            HtmlNodeCollection node2 =
            doc.DocumentNode.SelectNodes("//div[@class='forecastCity']//p[@class='date']");
            labelDate.Text = node2[0].InnerText;

            //Weather（天候）
            HtmlNodeCollection node3 =
            doc.DocumentNode.SelectNodes("//div[@class='forecastCity']//p[@class='pict']");
            labelWeather.Text = node3[0].InnerText;

            //High Temp（最高気温）
            HtmlNodeCollection node4 =
            doc.DocumentNode.SelectNodes("//div[@class='forecastCity']//ul[@class='temp']//li[@class='high']");
            labelTempHigh.Text = "最高気温 [前日差]： " + node4[0].InnerText;

            //Low Temp（最低気温）
            HtmlNodeCollection node5 =
            doc.DocumentNode.SelectNodes("//div[@class='forecastCity']//ul[@class='temp']//li[@class='low']");
            labelTempLow.Text = "最低気温 [前日差]： " + node5[0].InnerText;

            //Precip1[0-6]（降水確率）
            HtmlNodeCollection node6 =
            doc.DocumentNode.SelectNodes("//div[@class='forecastCity']//tr[@class='precip']//td");
            labelPrecip01.Text = node6[0].InnerText;

            //Precip1[6-12]（降水確率）
            labelPrecip02.Text = node6[1].InnerText;

            //Precip1[12-18]（降水確率）
            labelPrecip03.Text = node6[2].InnerText;

            //Precip1[18-24]（降水確率）
            labelPrecip04.Text = node6[3].InnerText;

            //WeatherPicture（天気画像）
            HtmlNodeCollection node7 = doc.DocumentNode.SelectNodes("//div[@class='forecastCity']//p[@class='pict']//img");
            var imageURL = node7[0].GetAttributeValue("src", "");

            System.Diagnostics.Debug.WriteLine(imageURL);

            //imageWeather.SetImageResource(Resource.Drawable.moon);

            var imageBitmap = GetImageBitmapFromUrl(imageURL);
            imageWeather.SetImageBitmap(imageBitmap);

        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }

    }
}