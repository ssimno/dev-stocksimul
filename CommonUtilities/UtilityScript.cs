using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace CommonUtilities
{
    public static class UtilityScript
    {
        #region 날짜 표현
        public static string TimeForToday(DateTime value)
        {
            DateTime today = DateTime.Now;
            DateTime timeValue = value;

            TimeSpan dateDiff = today - timeValue;

            int diffDay = dateDiff.Days;
            int diffHour = dateDiff.Hours;
            int diffMinute = dateDiff.Minutes;
            int diffSecond = dateDiff.Seconds;

            if (diffDay != 0)
            {
                if (diffDay >= 365)
                {
                    return $"{Math.Floor((decimal)diffDay / 365)}년전";
                }
                else if (diffDay >= 30)
                {
                    return $"{Math.Floor((decimal)diffDay / 30)}개월전";
                }
                else
                {
                    return $"{diffDay}일전";
                }
            }
            else if (diffHour != 0)
            {
                return $"{diffHour}시간전";
            }
            else if (diffMinute != 0)
            {
                return $"{diffMinute}분전";
            }
            else
            {
                return "방금전";
            }

        }
        #endregion

        #region 컨트롤 클론 생성
        public static T Clone<T>(T clone)
            where T : class
        {
            return XamlReader.Parse(XamlWriter.Save(clone)) as T;
        }
        #endregion

        #region 문자열로 Enum 타입 찾기
        public static T GetEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        #endregion

        #region 문자열로 메소드 부르기
        public static void CallMethod(FrameworkElement ctr, string method)
        {
            try
            {
                Type type = ctr.GetType();
                MethodInfo myClass_FunCallme = type.GetMethod(method, BindingFlags.Instance | BindingFlags.Public);
                if (myClass_FunCallme != null) myClass_FunCallme.Invoke(ctr, null);

            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }
        }

        public static void CallMethod(FrameworkElement _ctr, string _method, object _param)
        {
            try
            {
                Type type = _ctr.GetType();
                MethodInfo myClass_FunCallme = type.GetMethod(_method, BindingFlags.Instance | BindingFlags.Public);
                if (myClass_FunCallme != null) myClass_FunCallme.Invoke(_ctr, new object[] { });

            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }
        }

        public static void CallMethodParams(Window _context, string _instance, string _method, params object[] _params)
        {
            try
            {
                _params = _params == null ? new object[] { } : _params;
                FrameworkElement ctr = _instance.Equals("MainForm") ? _context : _context.FindName(_instance) as FrameworkElement;
                Type type = ctr.GetType();
                MethodInfo myClass_FunCallme = type.GetMethod(_method, BindingFlags.Instance | BindingFlags.Public);
                if (myClass_FunCallme != null) myClass_FunCallme.Invoke(ctr, _params.Length <= 0 ? null : _params.ToArray());
            }
            catch (Exception _e)
            {

                Console.WriteLine(_e.Message);
            }
        }

        #endregion

        #region 딜레이 콜
        public static List<System.Windows.Forms.Timer> timers = new List<System.Windows.Forms.Timer>();

        public static void DelayCall(float tick, Action call)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timers.Add(timer);
            EventHandler action = (object o, EventArgs e) => { call(); timer.Stop(); };
            timer.Tick += action;
            timer.Interval = (int)(tick * 1000);
            timer.Start();

        }
        public static void DelayCall(float tick, Action call, string tag)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timers.Add(timer);
            EventHandler action = (object o, EventArgs e) => { call(); timer.Stop(); };
            timer.Tick += action;
            timer.Tag = tag;
            timer.Interval = (int)(tick * 1000);
            timer.Start();

        }
        public static void DelayCall(int tick, Action call)
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timers.Add(timer);
            EventHandler action = (object o, EventArgs e) => { call(); timer.Stop(); };
            timer.Tick += action;
            timer.Interval = tick * 1000;
            timer.Start();

        }

        public static void DelayUpdate(int _tick, Action<System.Windows.Forms.Timer> _call)
        {

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timers.Add(timer);
            EventHandler action = (object o, EventArgs e) => { _call(timer); };
            timer.Tick += action;
            timer.Interval = _tick * 1000;
            timer.Start();

        }

        public static void DelayUpdate(float _tick, Action<System.Windows.Forms.Timer> _call)
        {

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timers.Add(timer);
            EventHandler action = (object o, EventArgs e) => { _call(timer); };
            timer.Tick += action;
            timer.Interval = (int)(_tick * 1000);
            timer.Start();

        }

        public static void DelayUpdate(float _tick, Action<System.Windows.Forms.Timer> _call, string _tag)
        {

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timers.Add(timer);
            EventHandler action = (object o, EventArgs e) => { _call(timer); };
            timer.Tick += action;
            timer.Tag = _tag;
            timer.Interval = (int)(_tick * 1000);
            timer.Start();

        }

        public static void DelayScheduleAllStop()
        {
            foreach (System.Windows.Forms.Timer timer in timers.ToArray())
            {
                timer.Stop();
                timer.Dispose();
            }
            timers.Clear();
        }

        public static void DelayScheduleStopByName(string _tag)
        {
            foreach (System.Windows.Forms.Timer timer in timers.ToArray())
            {
                if (timer.Tag != null)
                {
                    if (timer.Tag.Equals(_tag))
                    {
                        timer.Stop();
                        timer.Dispose();
                    }
                }

            }
        }


        #endregion

        #region 코드 처리속도 테스트
        public static void CodeTest(Action testFunc, Action<string> resultFunc)
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            testFunc();

            sw.Stop();


            resultFunc((sw.ElapsedMilliseconds / 1000.0F).ToString());
        }
        #endregion


        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        #region ** 오브젝트 배열 -> 해쉬테이블 변환
        public static Hashtable Hash(params object[] args)
        {
            Hashtable hashTable = new Hashtable(args.Length / 2);
            if (args.Length % 2 != 0)
            {
                Console.WriteLine("Error: Hash requires an even number of arguments!");
                return null;
            }
            else
            {
                int i = 0;
                while (i < args.Length - 1)
                {
                    hashTable.Add(args[i], args[i + 1]);
                    i += 2;
                }
                return hashTable;
            }
        }
        #endregion

        public static T FindChild<T>(DependencyObject parent, string childName)
                where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// 스트링으로 유저컨트롤 가져오는 메소드
        /// </summary>
        /// <param name="_name"> 유저컨트롤 스크립트 이름 </param>
        /// <returns></returns>
        public static UserControl GetUserControlByName(string _name)
        {
            //var controlType = Assembly.GetExecutingAssembly().GetTypes()
            var controlType = Assembly.GetCallingAssembly().GetTypes()
                .Where(a => a.BaseType == typeof(UserControl) && a.Name == _name)
                .FirstOrDefault();

            if (controlType == null)
                return null;

            return (UserControl)Activator.CreateInstance(controlType);
        }

        /// <summary>
        /// 스트링으로 유저컨트롤 가져오는 메소드 "다른 어셈블리에서"
        /// </summary>
        /// <param name="dllName"> dll이름 *확장자포함 </param>
        /// <param name="name"> 유저컨트롤 스크립트 이름 </param>
        /// <returns></returns>
        public static UserControl GetUserControlByNameOtherAssembly(string dllName, string name)
        {
            Assembly dll = Assembly.LoadFrom(dllName);
            var controlType = dll.GetTypes()
                .Where(a => a.BaseType == typeof(UserControl) && a.Name == name)
                .FirstOrDefault();

            if (controlType == null)
                return null;

            return (UserControl)Activator.CreateInstance(controlType);
        }

        /// <summary>
        /// 스트링으로 윈도우창 가져오는 메소드
        /// </summary>
        /// <param name="name"> 윈도우 스크립트 이름 </param>
        /// <returns></returns>
        public static Window GetWindowByName(string name)
        {
            //var controlType = Assembly.GetExecutingAssembly().GetTypes()
            var controlType = Assembly.GetCallingAssembly().GetTypes()
                .Where(a => a.BaseType == typeof(Window) && a.Name == name)
                .FirstOrDefault();

            if (controlType == null)
                return null;

            return (Window)Activator.CreateInstance(controlType);
        }



        /// <summary>
        /// 콤마 배열 to 스트링 배열
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<string> ConvertToCommaList(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return new List<string>();
            }
            return data.Split(',').ToList();
        }

        /// <summary>
        /// 스트링 배열 to 콤마 배열
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static string ConvertToCommaString(params string[] dataList)
        {

            return string.Join(",", dataList);
        }


        public static string InsertComma(double val)
        {
            return string.Format("{0:##,##0.##}", val);

        }

        public static string InsertComma(string val)
        {
            string val2 = Regex.Match(val.Replace(",", ""), @"(\d+\.\d+|\d+)").Value;
            return InsertComma(Convert.ToDouble(val2));
        }

        /// <summary>
        /// DLL 안에 있는 UC 읽어오기
        /// </summary>
        /// <param name="baseDirectory">AppDomain.CurrentDomain.BaseDirectory</param>
        /// <param name="viewFolderPath">bin부터 추가 경로</param>
        /// <returns></returns>
        public static Dictionary<string, UserControl> GetViewListByDLL(string baseDirectory, string viewFolderPath)
        {
            string path = baseDirectory + viewFolderPath;
            Dictionary<string, UserControl> viewList = new Dictionary<string, UserControl>();

            if (System.IO.Directory.Exists(path))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                //해당 폴더에 있는 파일이름을 출력
                foreach (var item in di.GetFiles())
                {
                    if (item.Extension.Equals(".dll"))
                    {
                        Assembly asm = Assembly.LoadFile(path + "\\" + item.Name);

                        Type[] tlist = asm.GetTypes();
                        foreach (Type t in tlist)
                        {
                            if (t.BaseType != null && t.BaseType.Name.Equals(nameof(UserControl)))
                            {
                                UserControl myControl;
                                try
                                {
                                    myControl = Activator.CreateInstance(t) as UserControl;
                                }
                                catch
                                {
                                    continue;
                                }
                                if (myControl != null)
                                {
                                    if (!viewList.ContainsKey(t.Name))
                                    {
                                        viewList.Add(t.Name, myControl);
                                    }
                                    else
                                    {
                                        Console.WriteLine("이름이 중복되는 화면이 존재합니다 : " + t.Name);
                                    }
                                }
                            }

                        }
                    }

                }

            }

            return viewList;
        }


        public static Dictionary<string, Func<UserControl>> UserControlFactories = new Dictionary<string, Func<UserControl>>();

        public static Dictionary<string, UserControl> GetViewListByDLL2(string baseDirectory, string viewFolderPath)
        {
            string path = baseDirectory + viewFolderPath;
            Dictionary<string, UserControl> viewList = new Dictionary<string, UserControl>();

            if (System.IO.Directory.Exists(path))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                //해당 폴더에 있는 파일이름을 출력
                foreach (var item in di.GetFiles())
                {
                    if (item.Extension.Equals(".dll"))
                    {
                        Assembly asm = Assembly.LoadFile(path + "\\" + item.Name);

                        Type[] tlist = asm.GetTypes();
                        foreach (Type t in tlist)
                        {
                            if (t.BaseType != null && t.BaseType.Name.Equals(nameof(UserControl)))
                            {
                                if (UserControlFactories.ContainsKey(t.Name))
                                {
                                    // Use cached instance if exists
                                    UserControl myControl = UserControlFactories[t.Name]();
                                    if (!viewList.ContainsKey(t.Name))
                                    {
                                        viewList.Add(t.Name, myControl);
                                    }
                                    else
                                    {
                                        Console.WriteLine("이름이 중복되는 화면이 존재합니다 : " + t.Name);
                                    }
                                }
                                else
                                {
                                    // Create a new instance and cache it
                                    UserControl myControl;
                                    try
                                    {
                                        myControl = Activator.CreateInstance(t) as UserControl;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    if (myControl != null)
                                    {
                                        UserControlFactories[t.Name] = () => myControl;
                                        if (!viewList.ContainsKey(t.Name))
                                        {
                                            viewList.Add(t.Name, myControl);
                                        }
                                        else
                                        {
                                            Console.WriteLine("이름이 중복되는 화면이 존재합니다 : " + t.Name);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            return viewList;
        }


        /// <summary>
        /// g.resources에 저장된 파일들을 메인 어셈블리로 복사하는 함수
        /// </summary>
        /// <param name="folderPath">복사 대상 폴더 경로 - (예시)Resources/Map</param>
        /// <param name="copyFolderPath">복사되는 폴더 경로</param>
        public static void CopyFilesToMainAssembly(Assembly assembly, string folderPath, string copyFolderPath)
        {
            // 현재 코드상 어셈블리 받기
            //Assembly ass = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName().Name + ".g.resources";
            string[] files;
            Stream stream = assembly.GetManifestResourceStream(resName);

            // g.resource 안에 있는 파일 이름 저장
            if (stream != null)
            {
                using (var reader = new System.Resources.ResourceReader(assembly.GetManifestResourceStream(resName)))
                {
                    files = reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();
                }

                foreach (var file in files)
                {
                    //Console.WriteLine("fileName = " + Path.GetFileName(file));

                    // 지정한 경로와 리소스에 저장된 파일의 경로 비교
                    if (folderPath.Equals(System.IO.Path.GetDirectoryName(file).Replace("\\", "/"), StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 파일의 URI 저장
                        Uri u = new Uri(GetResourcePathForAssembly(assembly.GetName().Name, $"{folderPath}/{System.IO.Path.GetFileName(file)}"));
                        StreamResourceInfo info = Application.GetResourceStream(u);
                        MemoryStream memoryStream = new MemoryStream();
                        info.Stream.CopyTo(memoryStream);
                        byte[] byteArr = memoryStream.ToArray();

                        // 메인어셈블리에 복사할 경로가 없을 경우 폴더 생성
                        DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(Environment.CurrentDirectory.ToString(), $"{copyFolderPath}"));
                        if (di.Exists == false) di.Create();

                        // 파일 생성
                        using (BinaryWriter writer = new BinaryWriter(File.OpenWrite($"{copyFolderPath}/{System.IO.Path.GetFileName(file)}")))
                        {
                            writer.Write(byteArr);
                        }
                    }
                }
            }


        }


        /// <summary>
        /// 클래스 라이브러리에서 사용하는 리소스 주소
        /// </summary>
        /// <param name="assemblyName">어셈블리 이름</param>
        /// <param name="folderPath">폴더 경로 및 파일 이름</param>
        /// <returns></returns>
        public static string GetResourcePathForAssembly(string assemblyName, string folderPath)
        {
            return $"pack://application:,,,/{assemblyName};Component/{folderPath}";
        }



        /// <summary>
        /// 공유폴더에 있는 Json 파일들 저장
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> ReadJsonFilesFromFolderAsync(string folderPath, string fileFullName)
        {
            Dictionary<string, string> jsonDataList = new Dictionary<string, string>();

            if (Directory.Exists(folderPath))
            {
                string[] jsonFiles = await Task.Run(() => Directory.GetFiles(folderPath, fileFullName));

                foreach (string file in jsonFiles)
                {
                    try
                    {
                        string fileName = System.IO.Path.GetFileName(file);
                        string fileContent = await ReadFileAsStringAsync(file);

                        jsonDataList.Add(fileName, fileContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Folder not found: {folderPath}");
            }

            return jsonDataList;
        }

        private static async Task<string> ReadFileAsStringAsync(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }


        /// <summary>
        /// 소수점 자리수 표현
        /// </summary>
        /// <param name="number"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static double TruncateDecimalPlaces(double number, int decimalPlaces)
        {
            double scaleFactor = Math.Pow(10, decimalPlaces);
            double scaledNumber = number * scaleFactor;
            double truncatedScaledNumber = Math.Truncate(scaledNumber);
            return truncatedScaledNumber / scaleFactor;
        }
    }
}
