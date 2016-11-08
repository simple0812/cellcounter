using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;

namespace EmguApp.libs
{
    public class WebBrowserHelper
    {

        public static readonly WebBrowserHelper Instance;
        static WebBrowserHelper()
        {
            Instance = new WebBrowserHelper();
        }


        //批量保存到独立存储空间
        public void SaveFilesToIsoStore()
        {
            string[] files = {
            "CreateProduct.html"
            };

            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (false == isoStore.FileExists(files[0]))
            {
                foreach (string f in files)
                {
                    StreamResourceInfo sr = Application.GetResourceStream(new Uri(f, UriKind.Relative));
                    using (BinaryReader br = new BinaryReader(sr.Stream))
                    {
                        byte[] data = br.ReadBytes((int)sr.Stream.Length);
                        SaveToIsoStore(f, data);
                    }
                }
            }
        }

        public void SaveFileToIsoStore(string path, string name, Stream stream)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(path))
                {
                    isoStore.CreateDirectory(path);
                }

                using (BinaryReader br = new BinaryReader(stream))
                {
                    byte[] data = br.ReadBytes((int)stream.Length);
                    //using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(path + "/" + name)))
                    //{
                    //    bw.Write(data);
                    //    bw.Close();
                    //}
                    using (IsolatedStorageFileStream isf = isoStore.OpenFile(path + "/" + name, FileMode.OpenOrCreate))
                    {
                        isf.Write(data, 0, data.Length);
                        isf.Close();
                    }
                }
            }

            stream.Close();
        }

        public void SaveFileToIsoStore(string fullPath, Stream stream)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            using (BinaryReader br = new BinaryReader(stream))
            {
                byte[] data = br.ReadBytes((int)stream.Length);

                using (IsolatedStorageFileStream isf = isoStore.OpenFile(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    isf.Write(data, 0, data.Length);
                    isf.Close();
                }
            }
        }

        public void SaveWebFileToIsoStore(string fileUri, string dir, string fileName, Action callBack)
        {
            WebClient wc = new WebClient();
            wc.OpenReadAsync(new Uri(fileUri));
            wc.OpenReadCompleted += (o, e) =>
            {
                if (e.Error == null)
                    WebBrowserHelper.Instance.SaveFileToIsoStore(dir, fileName, e.Result);

                callBack?.Invoke();
            };
        }

        public void SaveToIsoStore(string fileName, byte[] data)
        {
            string strBaseDir = string.Empty;
            string delimStr = "/";
            char[] delimiter = delimStr.ToCharArray();
            string[] dirsPath = fileName.Split(delimiter);

            //Get the IsoStore.
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            //Re-create the directory structure.
            for (int i = 0; i < dirsPath.Length - 1; i++)
            {
                strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                isoStore.CreateDirectory(strBaseDir);
            }

            //Remove the existing file.
            if (isoStore.FileExists(fileName))
            {
                isoStore.DeleteFile(fileName);
            }

            //Write the file.
            using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(fileName)))
            {
                bw.Write(data);
                bw.Close();
            }
        }

        //保存htm文件到独立存储空间
        public void InitHtml(string path, string name)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.Application,
                Type.DefaultBinder);

            if (!isoStore.FileExists(path + "/" + name))
            {


                if (!isoStore.DirectoryExists(path))
                {
                    isoStore.CreateDirectory(path);
                }

                StreamResourceInfo sr = Application.GetResourceStream(new Uri(path + "/" + name, UriKind.RelativeOrAbsolute));
                using (BinaryReader br = new BinaryReader(sr.Stream))
                {
                    byte[] data = br.ReadBytes((int)sr.Stream.Length);
                    using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(path + "/" + name)))
                    {
                        bw.Write(data);
                        bw.Close();
                    }
                }
            }

        }



        //保存与htm文件相关联的资源到独立存储空间
        public void InitResources(string path, string name)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (!isoStore.FileExists(path))
            {
                if (!isoStore.DirectoryExists(path))
                {
                    isoStore.CreateDirectory(path);
                }

                //component是必写的 PhoneApp2是项目文件名称
                StreamResourceInfo sr = Application.GetResourceStream(new Uri("/PhoneApp;component/" + path + "/" + name, UriKind.Relative));
                using (BinaryReader br = new BinaryReader(sr.Stream))
                {
                    byte[] data = br.ReadBytes((int)sr.Stream.Length);
                    using (BinaryWriter bw = new BinaryWriter(isoStore.CreateFile(path + "/" + name)))
                    {
                        bw.Write(data);
                        bw.Close();
                    }
                }
            }
        }
    }
}
