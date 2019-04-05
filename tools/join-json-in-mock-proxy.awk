# 简化 mock-proxy 的输出，防止输出内容太多影响 travis-ci 显示

BEGIN { FS="\\|\033\\[0m" ; line=""}

{
    # 只处理 mock-proxy 的日志
    if($1 ~ /\033\[[0-9]{2}mmock-proxy/) {
        if($2 ~ /^ \t\{/) {
            # json 开头
            line = "{"
        } else if($2 ~ /^ \t\}/) {
            # json 结尾

            # 简化 json 并移除 body 部分
            handle_json = "jq -c 'del(.. | .body?)'"
            print line "}" |& handle_json
            close(handle_json, "to")
            handle_json |& getline res
            close(handle_json)

            print $1 "|\033[0m\t" res

            line = ""
        } else if ($2 ~ /^\s*because:\s*$/) {
            line = $2
        } else if ($2 ~ /^\s*$/){
            # 一个空行
            if (line ~ /^\s*because:\s*$/) {
                # because 之后的第一个空行
                next
            } else if (line ~ /^\s*because:/) {
                # because 段落完成之后的空行
                print $1 "|\033[0m\t" line
                line = ""
            } else {
                #print $0
                # 跳过所有的空行
            }
        } else if (line != "") {
            # 中间部分
            line = line $2
        } else {
            # 其他内容
            print $0
        }
    } else {
        print $0
    }
}
