 
var WebSocketJsLib = {
 
        WebSocketInit: function(url){
            var init_url = Pointer_stringify(url);
            window.wsclient = new WebSocket(init_url);

        window.wsclient.onclose = function(evt) {
             console.log("[closed]");
        }; 
        window.wsclient.onopen = function (evt) {
            SendMessage('Multiplayer', 'OnConnect');
           console.log("[WebSocket Connected]");
        };
        window.wsclient.onmessage = function(evt) {
            SendMessage('Multiplayer', 'OnMessage', evt.data);
        }; 
        window.wsclient.onerror = function(evt) {
          
        };
    },
    State: function(){
       
        return window.wsclient.readystate;
    },
    WebSocketSend: function(msg){
        if ((typeof window.wsclient !== "undefined")&& (window.wsclient !== null)){
            window.wsclient.send(Pointer_stringify(msg));		
         
        }
    },
    Close: function(){
        if ((typeof window.wsclient !== "undefined")&& (window.wsclient !== null)){
            console.log('[close]');
            window.wsclient.close();
        }
    },
    Chat: function(msg){
	ReactUnityWebGL.chat(msg);
    },
}
mergeInto(LibraryManager.library, WebSocketJsLib);