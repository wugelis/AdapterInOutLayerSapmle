# 軟體架構設計：無題

![Hexagonal Architecture](https://i.imgur.com/bMOS6cv.png)
Logo 圖片、六角架構

取自：https://reflectoring.io/spring-hexagonal/

## 前言

為什麼叫做無題？因為軟體架構設計是一個相當龐大的主題，也不可能在一篇文章中談完，先前我也曾寫過許多架構設計系列文、包括領域驅動設計 DDD 與 整潔架構 Clean Architecture 與如何在以上的軟體架構為基礎來落實 TDD。

今天，我在以更廣泛與更實務的角度來探討軟體的架構設計，文之中也會搭上我這些年來所設計的框架、Framework 並對上也是這些年後來所讀的相關書籍、像是 DDD 領域驅動設計、Uncle Bob 的 整潔架構 Clean Architecture、還有正在讀的【Get You Hands Dirty on Clean Archiecture】，這這些書的洗禮之下，也在下面的文章中裡探討缺失、改善的方式，希望對於工作中正在進行架構設計或是正在進行專案規劃，但想要有個架構設計的資料可以參考的程式設計師，那麼或許您可以參考文章中的資料，或許對您有所幫助。


## 階層式架構設計的隱憂

圖（一）、常見的階層式架構
![階層式架構](https://i.imgur.com/TTJ1L8T.jpg)
傳統的階層式架構幾乎伴隨著我們好幾 10 年了，相信讀者不陌生，許多的老系統，包括我在 18 年前在流通業開發的幾個系統、以及後來10年前在X創開發設計的好幾個政府機關的系統也都是階層式架構設計。

究竟什麼是階層式架構？它與更早期講的三層式架構有些不同、比如 Windows DNA 那又是更早的跨機器架構，注意了，對是『跨機器』的軟架構，像是微軟早期 (DCOM, Distributed Component Object Model) 所以 DCOM 加上 Distribute 表示快機器、而 COM 就是跨行程。

這是早期的分散式環境，套用到現今，許多人想到的 Microservices/Docker.. 微服務等容器環境，這在跨機器的場景非常類似，但是 DCOM 需要完整的 OS 作業環境的支援、而 Microservices 微服務拜後來【VM 虛擬化技術】又再進階到【OS 作業系統虛擬化技術】之賜，使的應用程式可以包裹在更輕量化的容器環境之中，沒有作業系統啟動速度慢、占用系統支援較多等問題、更適合建構需要橫向擴展的 Cloud 雲端運算的環境中。

原歸正傳，所謂的階層式架構指的是近 10 多年來 Web 的興起，大部分系統都是 Web -> Application Server -> Database 的架構，前面的 Web 與 Application Server 在很多數企業環境中可能都在同一台，Database 當然會有獨立的伺服器主機，只不過在軟體系統中的 Data Access Layer 通常也在 Application Server 裡，而網站若運行在 IIS 上，用量不大的企業內部系統可能就放置在同一台上，有些情境因為機器的備援、或是效能，會建議拆不同的機器來執行，形成了 Three-Tier 架構。這些都是種階層式架構。

而階層式架構它一開始確實能夠切開 Web 與 Domain (領域層)[以下都簡稱 Domain] 以及 Persistence 的耦合性，因為它讓我們可以使用不同的 Web 架構而不影響 Domain 層或 Persistence，而實際存取 Database 的 Data Access Layer 似乎也能夠被各系統所共用，達到很高的共用性，如妥善的使用確實很好維護。

可是，問題來了，通常專案開發軟體不可能不修改、不維護啊！階層式架構開發最大的問題在於『資料庫導向開發』如果我今天只是要動一個 Persistence 的一個資料庫欄位 Column，這異動恐怕連同 Domain 與 Web 都要一起修改，這個架構非常容易走偏，因為建置 Domain 都得參考 Persistence 、修改 Domain 商業邏輯也得先修改你的 Persistence，很容易地到最後 Domain 與 Persistence 就牽扯在一塊。

底下，我們來看一種常見的階層式架構範例：

圖（二）、階層式架構範例
![階層式架構範例](https://i.imgur.com/Li2WrHC.jpg)

我仔細地來解說，這個系統分為前端 WebUI、領域層 (Domain.Employee)、與 儲存實體層 (PersistenceLayered) 這三層，Domain.Employee 會直接參考 PersistenceLayered，然後通常 WebUI 可能會同時參考 Domain.Employee 與 PersistenceLayered，且很多企業端會認為 WebUI必須同時參照兩者，甚至 WebUI 也需要同時安裝 PersistenceLayered 需要的 ORM 相關套件，像是 Entity Framework 等。

圖中的 Domain.Employee 的 EmployeeService.cs 其實比較像是過水的資料服務層，雖然 EmployeeRepository.cs 是定義在 PersistenceLayered 裡，但它的需求的變化完全相依 PersistenceLayered、或說會因為 PersistenceLayered 修改而跟著改變。表面上看起來是抽離出來的 Service 層，但事實上是完全的『資料驅動式設計』對吧？

這樣的程式碼是不是非常的常見？其實包括我 8-9 年以前撰寫的『分層架構系列 / 轉換成 MVC 系列文』都屬於此類。


## 單一職責原則（Single Resposibility Principle, SRP）

學習軟體開發或是本身是開發者的，應該都學習過物件導向五大設計原則 SOLID (SRP, OCP, LSP, ISP, DIP) 這五大設計原則，當中的 SRP (Single Reposibility Principle) 相信是再熟悉不過的了。

此原則告訴我們：『每個類別只需要做好一件事』

這概念在初淺不過，對於單一職責的表述也是我們常聽到的一種解釋，某方面來說，也非常容易理解。

而在［Clean Architecture 實作篇］裡，我覺得解釋更好，針對 SRP 的定義其實沒有這麼初淺有著詳盡的說明，其真正的定義應該是『每一個(元件/類別)應該只有一種被修改的理由』，而不是指『任務』或是『做什麼事情』，書中也打趣說該原則應該稱作［單一理由修改原則］才對，因為元件如果只有一種理由需要被修改，到最後這個元件往往也都只做一件事情，也許因為這樣，才讓許多人對於 SRP 有著就是指『只做一件事情的假象』吧！

書中也提到，如果某個元件只有一種被修改的理由，那麼這表示如果是其他理由的話，那麼都可以不用去修改這個元件，也就是系統本身的可維護性便會提高不少，因為獨立性提高，個元件各司其職，劃分的非常清楚，只是要注意的是『元件』會因為它的依賴方向使的它得要跟著修改，也就是說，元件多一個依賴，便會增加一種可能需要被修改的原因，因為依賴關係讓彼此受牽連。

圖（三）、元件的依賴關係
![元件的依賴關係](https://i.imgur.com/WVNOb5T.jpg)

如圖三，我們可以看到元件A與元件 B 或 C 或者 D 都有著依賴關係、而且還相依於 E，因此一旦 D 或者是 B 或 C 的修改都可能影響到 A，甚至讓 A 出現異常的情儣，反倒是 E 都沒有相依於其他元件，所以可以說 E 只有一種被修改的理由。對照到整潔架構裡，E 就像是 Domain Layer 最乾淨的部分，永遠只有一種裡有需要去修改它，因為它不與任何元件套件相依。

## 在 .NET Core 改良版的 Easy Architect Framework

因為長年來進行軟體專案開發，慢慢地自己察覺到階層式架構的一些缺點，於是藉由替某客戶發展 Framework 的機會，我希望發展出一種基於 Server Component 的元件架構，其實說穿了就是種部署架構，這個 Server Component 其實是一個乾淨的 C# Class，它不參考任何的外部組件、只單純提供一種 Server Method （伺服器方法）的架構，對開發專案的 Developer 而言，不需要太高的 Skill，只要需要知道 C# 怎麼寫 Class 以及如何撰寫 Method 即可開始做專案了，也就是專案開發的門檻可降低 + 提高系統穩定性 + 容易部署 + Developer 只需要專注在『領域邏輯 / 業務邏輯』即可。

當時的 Server Component 與 Server Method 基本結構範例如下：

    using EasyArchitect.BO.ServerComponent;
    using EasyArchitect.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;


    namespace Domain.Soft
    {
        /// <summary>
        /// BO Server Component
        /// <summary>
        [WriteLog(UseLogType.ToFileSystem)]
        [WriteExceptionLog(UseLogType.ToEventLog)]
        public class Service : ServerComponentBase
        {
            //請在Login Method裡面撰寫Login邏輯，此名稱與參數不可更改。
            /// <summary>
            /// Login邏輯
            /// </summary>
            /// <param name="UserInfo">使用者相關資訊</param>
            /// <returns></returns>
            [EnabledAnonymous(true)]
            [ExposeWebAPI(true)]
            public bool Login(UserInfo param)
            {
                return true;
            }

            /// <summary>
            /// 基本範例，取得目前時間
            /// </summary>
            /// <param name="param"></param>
            /// <returns></returns>
            [EnabledAnonymous(true), ExposeWebAPI(true)]
            public DateTime GetDateTime(decimal param)
            {
                return DateTime.Now;
            }
        }
    }

範例程式較為簡單，只有一個 Login() 方法與一個 GetDateTime() 方法，該框架的好處就是，Developer 只需要撰寫 C# Method 後，框架的 ApiHostBase 會自動將其開放為 Web API 方法，開發人員無須關心所有與 JSON 序列化相關的問題。

我以一張圖來演示這個架構。

圖（四）、Easy Architect 的 Server Component 架構圖
![Easy Architect 的 Server Component 架構圖](https://i.imgur.com/LyF87E7.jpg)

如圖中，我的 Server Componenet 的 Server Method 其實是透過 Reflection 來 Invoke 目標方法的，好處是充分發揮類似 Domain Object 的效果，因為中間隔著 Reflection 機制，所以 Server Component 與 Infrastructure/UI/Controller 可以完全讀隔絕開 + 完全沒有相依性。

我這邊擷取 ApiHostBase 的 Get 處理的部分程式碼：

    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Reflection;

    namespace Std20.EasyArchitect.ApiHostBase
    {
        /// <summary>
        /// ApiHostBase for .NET Standard 2.0
        /// </summary>
        [Route("api/[controller]/{dllName}/{nameSpace}/{className}/{methodName}/{*pathInfo}")]
        [Route("api/[controller]/{dllName}/{nameSpace}/{methodName}/{*pathInfo}")]
        [Route("api/[controller]/{dllName}/{methodName}/{*pathInfo}")]
        [Route("api/[controller]/{dllName}/{*pathInfo}")]
        [Route("api/[controller]/{*pathInfo}")]
        [ApiController]
        public class ApiHostBase: ControllerBase
        {
            /// <summary>
            /// 處理 Get 呼叫
            /// </summary>
            /// <param name="dllName"></param>
            /// <param name="nameSpace"></param>
            /// <param name="className"></param>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public ActionResult<object> Get(string dllName, string nameSpace, string className, string methodName)
            {
                if(string.IsNullOrEmpty(dllName) ||
                    string.IsNullOrEmpty(nameSpace) ||
                    string.IsNullOrEmpty(className) ||
                    string.IsNullOrEmpty(methodName))
                {
                    return GetJSONMessage("輸入的 Url 有誤！請確認！");
                }

                Assembly target = Assembly.Load($"{dllName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                if(target == null)
                {
                    return GetJSONMessage($"找不到名稱為 {dllName} 的 DLL，請確認該 DLL 有存在在 bin 資料夾中！");
                }

                object result = null;
                Type targetType = target.GetType($"{nameSpace}.{className}");
                object targetIns = Activator.CreateInstance(targetType);
                var methodResult = targetType.GetMethods(
                    BindingFlags.Default | 
                    BindingFlags.Public | 
                    BindingFlags.Instance)
                    .Where(c => c.Name.ToLower() == methodName.ToLower())
                    .FirstOrDefault();

                if(methodResult == null)
                {
                    return GetJSONMessage($"找不到名稱為 {dllName} 的方法名稱，請確認該 DLL 有存在該 public 的 {methodName} 名稱！");
                }

                var queryString = Request.Query;
                
                if(queryString.Count() > 0)
                {
                    ParameterInfo[] psInfo = methodResult.GetParameters();
                    if(psInfo.Count() > 0)
                    {
                        Type psType = psInfo[0].ParameterType;
                        object paramIns = Activator.CreateInstance(psType);
                        PropertyInfo[] properties = psType.GetProperties();

                        foreach (var q in queryString)
                        {
                            string keyName = q.Key;
                            
                            queryString.TryGetValue(q.Key, out var keyValue);
                            
                            var paramInsResult = properties
                                .Where(c => c.Name.ToLower() == keyName.ToLower())
                                .FirstOrDefault();

                            if(paramInsResult != null)
                            {
                                if(paramInsResult.PropertyType == typeof(int))
                                {
                                    paramInsResult.SetValue(paramIns, Convert.ChangeType(keyValue.ToString(), paramInsResult.PropertyType));
                                }
                            }
                        }
                        result = methodResult.Invoke(targetIns, new object[] { paramIns });
                    }
                }
                else
                {
                    result = methodResult.Invoke(targetIns, null);
                }

                return result;
            }

            private ActionResult<object> GetJSONMessage(string message)
            {
                return new string[] { message }
                    .Select(c => new
                    {
                        Err = c
                    }).ToList();
            }
        }
    }

這雖然只是部分程式碼，不過也算是 Get 的核心處理部分，Post 部分就不方便提供，從程式碼裡可以看出來，它幾乎能夠處裡各種型別，型別當然是由 Target 的 object/method 裡的 ParameterType 所提供，好處是非常靈活，Server Component 也可乾淨。

因為 Server Component 如同 Domain Layer 只需要乾淨的 Class 負責描述一個充血物件，但如果仔細對照書中解釋，這個架構也仍然沒有完全遵循 Clean Architecture 的精神，怎麼說呢？

第一是，Server Method 並沒有一種 Adapter (in/out) 機制［六角架構術語］的設計，

第二是、如果 Server Method 回傳了 ORM 的 Entity 回來，就會導致 Server Component/Domain Object 相依外圈的 Persistance Layer 了。

第三是、我並未提供依賴反轉機制、也未提供 DI 依賴注入的功能在裡面。

第四是、Server Componenet 有一個父類別、這增加了 Domain 複雜性，還不夠純淨。

所以看似美好的開發，還是很容易一個不小心就陷入書中所提及的『不小心就偷吃步了』。

針對這些問題，我後來在 .NET Core 上另外實作了一個全新的版本，這個版本可以讓 Server Component 真的是非常乾淨的 Class ，如下範例程式：

    using EmployeeViewObjects;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    namespace HelloWorldBO
    {
        public class EmployeeService
        {
            public string GetHelloWorld()
            {
                return "Hello World for Web API Framework 第三梯！";
            }

            public IEnumerable<EmployeeVO> GetEmployees(MyHelloWorldVO input)
            {
                return new EmployeeVO[] {
                    new EmployeeVO() { EmpId = 1, EmpChtName = $"Gelis_{input.Test}", Title = "工程師"},
                    new EmployeeVO() { EmpId = 2, EmpChtName = "Allan", Title = "工程師"}
                };
            }
        }
    }

這個做法其實就是我在 3 年開課講的 跨平台的 Web API Framework 課程的實作內容。

圖（五）、Gelis - 程式設計訓練營 - 跨平台的 Web API Framework 框架開發（第三梯）
![Gelis - 程式設計訓練營 - 跨平台的 Web API Framework 框架開發（第三梯）](https://i.imgur.com/rw1dbG5.jpg)

當時課程連結：https://mystudyway.kktix.cc/events/softshare-web-api-framework-third

這個課程時做的基本範例雛形有上在 NuGet Packages Marget 市集上。

連結：https://www.nuget.org/packages/StdEasyArchitect.Web.WebApiHostBase/

有興趣的可以下載測試看看。

## 如何在整潔架構上落實 Unit Test？

前面談完我在專案實作上的架構有哪些並未遵循 Clean Architecture 的精神，現在，我們來談談測試好了，書中的第七章有提及架構測試，而這便我針對『領域實體』的單元測試來做討論，因為測試的方法論如果是談 TDD 那真是博大精深，先前我有一篇文章：『您的軟體架構夠敏捷嗎？（三）- 使用 TDD 實現最後的設計』便是探討這個部分，透過測試來分析需求應該是軟體開發中掌握設計同時使程式碼隨時重構在輕量可測試、高可維護性的唯一解法了，只是怎麼搭配 Clean Architecture 呢？

這裡我參照書籍中大量的六角架構術語，首先是『先 Package by feature 後、再 Package by Layer』，這邊由於我是使用 .NET 6，因為在 .NET 環境中沒有 Package 的概念，使用的是 NameSpace 來提供類似 Java 的 Package，不過在 .NET 還可以直接拆不同的 Assembly，將 Domain Layer 獨立出來，並斷開 Domain Layer 與 Application Layer 彼此的耦合性，並塑造出由外而內的依賴環境。

在書中所提的 Use Case Layer 就是 Application Layer ，而書中是使用六角架構，所以這裡我仿造書中的 Java 範例程式並想要重新以 .NET 6 來撰寫相同的範例，只是我在 Account 這個 Entity 加油添醋增加一個 CheckIsAID() 方法，以便達成對 CheckIsAID() 這個方法演示一下對 Domain Layer 進行單元測試，爾後再對書中其他如 ISendMoneyUseCase 介面進行隔離測試（題外話：我覺得書中對於測試的章節所撰寫的範例我認為比較像是整合測試  或 End2End 的測試，而我會比較想『只針對【邏輯】做單元測試』）。

當天傍晚試一下手感 🤣，使用 C# 撰寫書中的範例，而這個範例就像是我先前的 Clean Architecture 範例一樣，我只需要稍作修改，加上六角架構的術語，大約 20 分鐘可以寫出一個樣板。

比如說，在六角架構中 Port 代表存放『對（外層）的依賴反轉的介面』，而所謂的 Port 又可分 In Port 和 Out Port，所謂的 In/Out 是從 Application Service 的位置來看，如果這個介面是由外層如 Web UI 或 controller 由外往內來呼叫內層，那它就是一個 In port，如果是由 Application Service 的的物件或內層 Entity 來呼叫外層，例如像是外部資料層的 Repository，那麼他就是一個 由內而外的 Out Port 如果它是讓 application layer的物件呼叫外部服務的介面，例如用來存取資料庫的repository，這就是一個out port（由內往外）
以這個 C# 樣板來說，我們可以將 In/Out 放置在由 Application 這個 Package 區隔開的物件中，而在 C# 中我們可以將這裡以 Assembly 區隔、而 Web 起始的 Package 因為由 adapter 起始，所以可以看做是 『由外而內 In』所以 Web 在外圈，可以獨自建一個 ASP.NET Core 6 的網站，且 adapter 下的 Out 因為是由 application service 『由內而外』的向外存取 Database 所以 Persistence 為外圈的 Infrastructure 的 資料層 Repository，所以完成 C# 程式碼如下圖。


圖（六）、以 .NET 6 重建書中該六角架構範例

![以 .NET 6 重建書中該六角架構範例](https://i.imgur.com/t3bEqTP.jpg)

這裡我使用 .NET Core/6 的內建 DI 就可以完成 DIP 反向依賴【In/Out Port】，這段程式碼會撰寫在 WebAccountUI 的 Program.cs 裡，並使用在 C# 9.0 新增加的 Top-Level Statement 的語言特性。

程式碼如下：

    builder.Services.AddScoped<ISendMoneyUseCase, SendMoneyService>();
    builder.Services.AddScoped<ILoadAccountPort, AccountPersistenceAdapter>();

依照書本中的介紹，我以 C# 來實作 In/Out 的 Application 這個 Assembly 中，這裡讀者會注意到我用 Assembly 取代這裡 Java 的 Pacakge or JAR，因為在 .NET 裡可透過不同的 Assembly 切開組件彼此的相依，當然組件內部有 references 可相依它需要的 DLL/Assembly，在 .NET 裡 Assembly 為可獨立存在的，這裡做為區隔開 + 並讓 Application Service / Domain Layer 均為獨立存在的 Assembly。

再透過 builder.Services 以 Scopd 註冊之後，我就可以從外層的 WebAccountUI 層的 AccountController 注入我定義在 Application Service 的 ISendMoneyUseCase 介面與其實作 SendMoneyService 類別 與 定義在 Persistence 的 AccountPersistenceAdapter 類別與 Application 的 Input Port 下的 ILoadAccountPort 介面。

這裡我在 WebAccountUI 的 AccountController 裡撰寫如下程式碼：

    using Application.port.In;
    using Application.Port.Out;
    using Domain;
    using Microsoft.AspNetCore.Mvc;
    using WebAccountUI.Models;

    namespace WebAccountUI.Controllers
    {
        public class AccountController : Controller
        {
            private readonly ISendMoneyUseCase _sendMoneyUseCase;
            private readonly ILoadAccountPort _loadAccountPort;

            public AccountController(ISendMoneyUseCase sendMoneyUseCase, ILoadAccountPort loadAccountPort)
            {
                _sendMoneyUseCase = sendMoneyUseCase;
                _loadAccountPort = loadAccountPort;
            }
            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Login(AccountViewModel accountViewModel)
            {
                // DIP 示例
                Account account = new Account();
                account.SetAccountId("F123456789");

                // Login 作業
                // ..略

                // 更新 Account 資訊
                _loadAccountPort.UpdateAccount(account);

                return View(accountViewModel);
            }

            public IActionResult ListAccount()
            {
                var listAccounts = _sendMoneyUseCase.ListAccounts();

                return View(listAccounts);
            }
        }
    }


這裡我們可以來測試一下，在 AccountController 是否能透拿到實體。

圖（七）、偵錯 WebAccountUI 專案
![偵錯 WebAccountUI 專案](https://i.imgur.com/FDm69hj.jpg)

這裡我們經過測試也能夠呼叫到 AccountPersistenceAdapter 類別，因為我 Login 時，最後會更新 Account 資訊，也就是會呼叫 _loadAccountPort.UpdateAccount(account); 這時，就會初始化 AccountPersistenceAdapter 並叫用 UpdateAccount 方法。

如下圖呼叫的情況，此為一個示例。

圖（八）、WebAccountUI 的 AccountPersistenceAdapter 的 UpdateAccount 方法被呼叫
![WebAccountUI 的 AccountPersistenceAdapter 的 UpdateAccount 方法被呼叫](https://i.imgur.com/uI7cT8w.jpg)


想看更多的範例程式碼，請參考我 Github 上這個範例的原始碼：
https://github.com/wugelis/AdapterInOutLayerSapmle

單元測試的作法留著下一次更新，我來讓各位體演一下在 Domain Layer 在最乾淨的情況下，如何『只測試邏輯？』

之後再向各位演示這種測試方法。

... 我先賣個關子...

持續更新

## 待續

