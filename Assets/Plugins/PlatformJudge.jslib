
mergeInto(LibraryManager.library,
 {
     HelloFloat: function()
     {
     var userAgentInfo = navigator.userAgent;      
        var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];    
        var flag = true;       
        for (var v = 0; v < Agents.length; v++)
        {        
         if (userAgentInfo.indexOf(Agents[v]) > 0)
         {   
          return 1; //如果是手机端就返回1
           break;       
         }     
        }       
        return 2; //如果是pc就返回2
    },
     });
