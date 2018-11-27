using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Index;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using z.Foundation;

namespace com.star.Logic.Search
{
    public class BuiltIndex_SingleThread
    {
        PerFieldAnalyzerWrapper analyzer;
        IndexWriter indexWriter;
        DataSource dataSource = new DataSource();

        DateTime dtNow = DateTime.Now;
        string strMainDirectory = "";
        string strCurrentIndex;

        /// <summary>
        /// 创建索引
        /// </summary>
        public void StartBuild()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (Init())
            {
                //添加索引
                //AddDocument();
                End();
            }
            else
            {
                Logger.Error("Initialization fail");
            }

            watch.Stop();
            //Logger.Info(string.Format("Build Item Index take time {0} ms", watch.ElapsedMilliseconds));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private bool Init()
        {
            try
            {
                strMainDirectory = string.Format("{0}\\Search\\XIndex", Constant.FILE_SERVER);
                strCurrentIndex = DateTime.Now.ToString("yyyyMMddHHmmss");

                //初始化数据源
                GetDataSource();

                //初始化分词器
                PanGu.Segment.Init(Constant.PanGuXML);
                analyzer = new PerFieldAnalyzerWrapper(new PanGuAnalyzer());
                analyzer.AddAnalyzer("AllCategoryId", new WhitespaceAnalyzer());
                //analyzer.AddAnalyzer("ShippingFromId", new WhitespaceAnalyzer());
                //analyzer.AddAnalyzer("OriginPlaceId", new WhitespaceAnalyzer());
                //analyzer.AddAnalyzer("BusinessModelId", new WhitespaceAnalyzer());

                //初始化IndexWriter
                Directory directory = FSDirectory.Open(new System.IO.DirectoryInfo(string.Format("{0}\\{1}", strMainDirectory, strCurrentIndex)));
                indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
                indexWriter.SetSimilarity(new XSimilarity());
                indexWriter.SetMaxBufferedDocs(10000);
                indexWriter.MaxMergeDocs = 10;//.SetMaxMergeDocs(10);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Initialization fail：" + ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        ///优化并关闭写索引库对象、更新当前索引库标识、删除过期索引库、主动清除占用内存
        /// </summary>
        private void End()
        {
            try
            {
                analyzer.Dispose();
                indexWriter.Optimize();
                indexWriter.Dispose();

                System.IO.File.WriteAllLines(string.Format("{0}\\currentIndex.txt", strMainDirectory), new string[] { strCurrentIndex });

                string[] dirs = System.IO.Directory.GetDirectories(strMainDirectory);
                foreach (var obj in dirs)
                {
                    if (System.IO.Directory.Exists(obj))
                    {
                        if (DateTime.Now.Subtract(System.IO.Directory.GetLastWriteTime(obj)).TotalDays > Constant.IndexExpireInterval)
                        {
                            System.IO.Directory.Delete(obj, true);
                        }
                    }
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取索引数据源
        /// </summary>
        /// <returns></returns>
        private DataSource GetDataSource()
        {
            var dataSource = new DataSource();

            return dataSource;
        }
    }
}
