 
var WebSocketJsLib = {
 
        WebSocketInit: function(url){
            var init_url = Pointer_stringify(url);
            window.wsclient = new WebSocket(init_url);
            window.wsclient.onopen = function(evt){ 
            console.log("WebSocket connected");
        }; 
        window.wsclient.onclose = function(evt) {
            SendMessage('Multiplayer', 'OnMessage', evt.data);
        }; 
        window.wsclient.onmessage = function(evt) {
            SendMessage('Multiplayer', 'OnMessage', evt.data);
        }; 
        window.wsclient.onerror = function(evt) {
            SendMessage('Multiplayer', 'OnMessage', evt.data);
        };
    },
    State: function(){
       
        return window.wsclient.readystate;
    },
    WebSocketSend: function(msg){
        if ((typeof window.wsclient !== "undefined")&& (window.wsclient !== null) && (window.wsclient.readystate != 0))
            window.wsclient.send(Pointer_stringify(msg));		
    },
    Close: function(){
        if ((typeof window.wsclient !== "undefined")&& (window.wsclient !== null))
            window.wsclient.close();
    }
}
mergeInto(LibraryManager.library, WebSocketJsLib);