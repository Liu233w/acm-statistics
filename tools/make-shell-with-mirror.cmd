REM 一个设定了部分环境变量的 shell，构建出的镜像将通过国内的源来安装依赖，用于提高开发时的构建速度

set APK_MIRROR=1
set NPM_MIRROR=1

cd ..
cmd