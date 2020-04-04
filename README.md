###RequestFilter-for-.NET-Core

在RequestFilterMiddlewareExtensions裡的
DefaultWhiteListIpCollection可以新增白名單
```
 /// <summary>
        /// 預設的 IP 白名單
        /// </summary>
        private static IEnumerable<string> DefaultWhiteListIpCollection => new[]
        {
        "::1",
        "127.",
        "192.",
        "10.",
        "172."
    };
```
不在白名單裡的會被導頁，
DefaultRestrictPathCollection限制路徑名稱如(swagger,nanoprofiler..等等)
```
 /// <summary>
        /// 預設的限制路徑名稱
        /// </summary>
        private static IEnumerable<string> DefaultRestrictPathCollection => new[]
        {
        "coreprofiler",
        "nanoprofiler",
        "swagger",
        "health"
    };
```