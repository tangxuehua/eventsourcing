eventsourcing
=============

This is an open-source framework which implement the event sourcing pattern and CQRS architecture and is suitable for developing DDD based applications.

1. 运行源代码前先执行命令build nuget以获取外部依赖的程序集。具体步骤：在当前README.md文件所在目录，按住shit键，然后鼠标右键，在弹出的菜单项中选择“在此处打开命令窗口”，然后输入：build nuget，然后回车即可。注意：你的目录中不能包含空格，否则会出错！
2. 如果要调试代码，需要先新建一个SQL数据库，比如名称叫EventSourcingSampleDB，然后在新建的数据库上执行SQL脚本：scripts\TableGenerateSQL.sql
3. 修改数据库连接字符串，配置文件目录：src\Sample\EventSourcing.Sample.Test\ConfigFiles\Debug\nhibernate.config，修改文件中key=connection.connection_string的配置值即可
4. 运行代码前需要安装MSMQ，否则会报错。要安装MSMQ很简单，到控制面板->所有程序->启动或关闭Windows功能，然后选择MSMQ进行安装即可。

补充说明：
Sample中EventSourcing.Sample.Test，EventSourcing.Sample.Host这两个工程都是顶层应用。
EventSourcing.Sample.Test用于以单元测试的方式来调用领域模型执行业务逻辑，产生Event Sourcing模式中的可溯源事件；
EventSourcing.Sample.Host是一个宿主控制台应用程序，作为异步事件总线的事件订阅者端点，它会以异步的方式消费由Test工程产生的可溯源事件；

所以运行代码时，如果你希望CQRS架构中的显示表中都有数据，那需要在运行Test工程前先运行EventSourcing.Sample.Host这个控制台应用程序。


## License

![GPL](http://www.gnu.org/graphics/gplv3-127x51.png)

	[GPL](http://www.gnu.org/copyleft/gpl.html)
	

	Copyright (C) 2012  CodeSharp

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.