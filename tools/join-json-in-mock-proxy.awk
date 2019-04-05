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
        } else if (line != "") {
            # json 中间
            line = line $2
        } else {
            # 非 json
            print $0
        }
    } else {
        print $0
    }
}
