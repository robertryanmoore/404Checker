using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _404Checker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            button1.Text = "Working...";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            button1.Text = "Working...";

            string[] links = fileContent.Split(
                 new[] { Environment.NewLine },
                 StringSplitOptions.None
                  );

            for (int i = 0; i < links.Length; i++) {

                try
                {
                    if (!string.IsNullOrEmpty(links[i]))
                    {
                        UriBuilder uriBuilder = new UriBuilder(links[i]);
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uriBuilder.Uri);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            links[i] = links[i] + "Broken - 404 Not Found";
                        }
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            links[i] = links[i] + " " + "URL appears to be good.";
                        }
                        else //There are a lot of other status codes you could check for...
                        {
                            links[i] = links[i] + " " +  string.Format("URL might be ok. Status: {0}.",
                                                       response.StatusCode.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    links[i] = links[i] + " " + string.Format("Broken- Other error: {0}", ex.Message);
                }

                Console.WriteLine(links[i]);
            }

            System.IO.File.WriteAllLines(@filePath.Substring(0, filePath.LastIndexOf("\\")+1) + "TestedLinks.txt", links);
            button1.Text = "Done";
        }
    }
}