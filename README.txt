金光集团官网 - 本地运行版
=============================

一、双击运行（推荐）
---------------------

【Windows 用户】
双击 "run_windows.bat"，自动安装依赖并启动服务器。

【Mac / Linux 用户】
终端运行：bash run_mac.sh


二、打开浏览器
----------------
启动后，在浏览器打开：

前台网站：http://localhost:5000
后台管理：http://localhost:5000/admin
后台账号：admin / sinarmas2024

三、停止服务器
----------------
按 Ctrl+C 即可停止。


四、如遇问题
----------------
1. "Python 未找到" → 去 https://www.python.org/downloads/ 下载安装
2. 端口被占用 → 命令行输入：python run.py （自动换端口）
3. 图片不显示 → 确认 static/images/ 文件夹有图片文件

五、部署到腾讯云
----------------
1. 把整个文件夹上传到腾讯云服务器
2. 安装 Python + Flask
3. 运行：python run.py --port=80
4. 配置 Nginx 反向代理 + SSL 证书

六、修改后台密码
----------------
打开 run.py，找到：ADMIN_PASS_HASH = ...
改为新密码的 MD5 值即可。
在线 MD5 工具：https://md5.cn
