# EasyGame

实现一个简单的IOC容器，划分了View、Model、System、Command、Event、Utility层级。
- 资源管理：实现一个资源加载框架，通过统一接口实现编辑器和AB包的资源加载、卸载、预加载、异步加载，对资源实现引用计数
- UI管理：在底层资源加载框架基础上开发的基于栈开发的UI框架，通过将面板压栈和出栈的方式控制面板。面板类提供面板的初始化、进入、暂停、继续、退出等接口
- 场景管理：将代码与场景进行绑定，为场景进入和退出提供了统一接口
- AB包打包工具和程序打包工具：通过设定的基础资源文件夹和预制体文件夹等一键打AB包、一键导出项目
