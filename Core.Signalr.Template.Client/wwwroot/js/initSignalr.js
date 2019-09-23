/**
 * 初始化连接
 * @param {object} option 参数
 * @param {string} option.url 连接的url地址
 * @param {string} option.loggingLevel 日志级别,默认为 Error
 * @param {number} option.delay 延迟连接 默认为3000毫秒
 * @param {function} option.onStarted 启动时触发
 * @param {function} option.onLine 启动时触发
 * @param {function} option.offLine 启动时触发
 * @returns {object} 连接的实例
 */
function initSignalr(option) {
    var config = Object.assign(true, {
        loggingLevel: signalR.LogLevel.Error,
        delay: 3000,
        url: ''
    }, option);

    var connection = new signalR.HubConnectionBuilder()
        .configureLogging(config.loggingLevel)
        .withUrl(config.url, {
            accessTokenFactory: option.accessTokenFactory
        })
        .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
        .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
        .build();

    connection.onreconnecting(function (info) {
        console.info('----------------------------------signalr-- onreconnecting', info);
    });

    connection.onclose(function (err) {
        console.info('--------------------------------signalr-- onclose', err);
    });

    connection.on('OnNotify', config.onNotify);

    connection.on('OnLine', config.onLine);

    connection.on('OffLine', config.offLine);

    setTimeout(function () {
        connection.start().then(function (data) {
            option.onStarted && option.onStarted(data);
        }).catch(function (error) {
            console.error(error.toString());
        });
    }, option.delay);

    return connection;
}
