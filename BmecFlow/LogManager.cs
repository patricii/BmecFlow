﻿using System.IO;
using System.Windows.Forms;

namespace BmecFlow
{
    class LogManager
    {
        string pathName = @"Q:\quality_data\test_results";
        string strLogPattern = "*.rslt*";
        string dirError = "diretório e/ou arquivo não encontrados!!!";

        SQLManager sqlManager = new SQLManager();

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            //file is not locked
            return false;
        }
        public void getPatternAndWriteToDB(string line)
        {
            string[] tempSplit = line.Split(',');
            string logTrackId = tempSplit[1];
            string stationType = tempSplit[4];
            string passFail = tempSplit[9];

            if (logTrackId.Length == 10)
                sqlManager.InsertToMdb(logTrackId, stationType, passFail);
        }

        public void getLogResult()
        {
            FileInfo fileInfoMQS;
            try
            {
                foreach (string file_name in Directory.GetFiles(pathName, strLogPattern, SearchOption.TopDirectoryOnly))
                {
                    fileInfoMQS = new FileInfo(file_name);
                    if (!IsFileLocked(fileInfoMQS))
                    {
                        using (var reader = new StreamReader(file_name))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains("5GFR1BdTst"))
                                    line = line.Replace("5GFR1BdTst", "T5GFR1BdTst");

                                if (line.Contains("TH4") && !line.Contains("LEAK") && !line.Contains("L2VISIONCAL") && !line.Contains("L2ARNORM") && !line.Contains("L2ARGEN") && !line.Contains("RadioTst_VisCal") && !line.Contains("RGB_TEST") && !line.Contains("RadioTst_V"))
                                    getPatternAndWriteToDB(line);
                            }
                        }
                    }
                }
            }
            catch
            { }
        }
        public void UnitTrackingGenTxt(string folderName, string trackingInfos, string fileNameTrackId)
        {
            string dirName = FormMain.trackingDir + "\\" + folderName;
            Directory.CreateDirectory(dirName);

            string filepath = dirName + "\\" + fileNameTrackId + ".tracking";
            try
            {
                string logString = trackingInfos;
                if (!File.Exists(filepath))
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
                    {
                        writer.WriteLine(logString);
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Append, FileAccess.Write)))
                    {
                        writer.WriteLine(logString);
                    }
                }
            }
            catch
            {
                MessageBox.Show(dirError + fileNameTrackId);
            }
        }

        public void writeResult(string productName, string trackId, string stationName, string result, string valueResult)
        {
            string dirName = FormMain.LeakResultDir + "\\" + productName;
            Directory.CreateDirectory(dirName);

            string filepath = dirName + "\\" + productName + "_" + stationName + ".txt";
            try
            {
                string logString = productName + "," + trackId + "," + result + "," + stationName + "," + valueResult;
                if (!File.Exists(filepath))
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
                    {
                        writer.WriteLine(logString);
                    }
                }
                else
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Append, FileAccess.Write)))
                    {
                        writer.WriteLine(logString);
                    }
                }
            }
            catch
            {
                MessageBox.Show(dirError + trackId);
            }
        }

    }
}
