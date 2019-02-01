
$(function(){
 
  var jsonArray = [], flattenedJsonArray = [], flattenedAndFilterdJsonArray = [], 
  fullRenderedlist = [], flattenedRenderedlist = [], flattenedAndFilteredRenderedlist = [];
  
  $.getJSON('nav.json', function(data) {       
    jsonArray = data;  
  }).done(function(){
    
    // Full tree list - not flattened        
    renderedlist = buildListFromJsonArray(jsonArray);      
    
    // Flattened JSON and rendered list
    flattenJsonArray(jsonArray, flattenedJsonArray, "Plan");  
    flattenedRenderedlist = buildListFromJsonArray(flattenedJsonArray);   
    
    // Flattened and filtered rendered list
    flattenedAndFilterdJsonArray = $.grep(flattenedJsonArray, function(item, index){
      return item.name.indexOf("Lvl.2") !== -1;
    });
    
    flattenedAndFilteredRenderedlist = buildListFromJsonArray(flattenedAndFilterdJsonArray);   
   
    $(document.body).append(renderedlist);
    $(document.body).append(flattenedRenderedlist);
    $(document.body).append(flattenedAndFilteredRenderedlist);    
  });   

});

// Flattens an array using recursion.
function flattenJsonArray(originalArray, flattenedArray, propertyArrayName) {
    if (!originalArray) return;
    
    $.each(originalArray, function (index, item) {
        if (item[propertyArrayName].length === 0) {
            flattenedArray.push(item);
        } else {
            var newItem = jQuery.extend(true, {}, item);
            newItem[propertyArrayName] = [];
            flattenedArray.push(newItem);
            flattenJsonArray(item[propertyArrayName], flattenedArray, propertyArrayName);
        }
    });
}

function buildListFromJsonArray(data, $linkObject, levelDepth) {    
    if (typeof(data) !== 'object') return;
            
    var list = $('<ul />');
    
    var curDepth;
    if(levelDepth){
      curDepth = parseInt(levelDepth, 10);
      list.addClass("level" + curDepth);
    }else{
      curDepth = 0;
    }
    
    if($linkObject){
      list.append($linkObject);
    }
    
    $.each(data, function(index, item) {           
      var listItem = $("<li />");
      
      // Add classes for first or last list item
      if(index === 0){
        listItem.addClass("first");
      }else{
        if(index === (data.length - 1)){
          listItem.addClass("last");
        }
      }
      
      // Add link
      var link = $("<a />");
      link.attr("href", item.url);
      link.text(item.name);
      listItem.append(link);            
      
      if(item.children && item.children.length > 0){
        listItem.addClass("hasList");
        
        // Add show children button
        var showChildrenLink = $("<span />");
        showChildrenLink.addClass("showChildren");
        showChildrenLink.text(" > ");
        listItem.append(showChildrenLink);
        
        var navBackLink= $("<span />");
        navBackLink.addClass("backLink");
        navBackLink.text(" < ");                
                
        navBackLink = navBackLink.add(link.clone());
        
        var newDepth = curDepth + 1;
        listItem.append(buildListFromJsonArray(item.children, navBackLink, newDepth));
      }      
      list.append(listItem);       
    });    
    
    return list;    
}
