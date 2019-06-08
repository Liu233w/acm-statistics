const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

    if (!username) {
        throw new Error('请输入用户名')
    }

    const res = await request
        .get('http://loj.ac/find_user')
        .query({ nickname: username })

    if (!res.ok) {
        throw new Error(`Server Response Error: ${res.status}`)
    }
    var $ = cheerio.load(res.text);

    if ($('.header').filter((i, el) => $(el).text().trim() === '无此用户。').length >= 1) {
        throw new Error('用户不存在')
    }
    try {
        var acList = new Array();
        $('[href^="/problem/"]').filter(function (i, el) {
            acList.push($(el).text().trim());
        });
        var html = res.text.replace(/\s/gi, '');
        var submitDetailArr = html.match(/data:\[(\d+),(\d+),(\d+),(\d+),(\d+),(\d+),\]/i);
        var subCount = 0;
        for (var i = 1; i <= 6; i++) {
            subCount += parseInt(submitDetailArr[i]);
        }
        return {
            submissions: subCount,
            solved: parseInt(submitDetailArr[1]),
            solvedList: acList
        };
    }
    catch (e) {
        throw new Error('无法解析数据')
    }

}