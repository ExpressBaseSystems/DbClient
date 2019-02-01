var SqlEval = function () {
    var str = '';
    var index = 0;
    var row = 0;
    var idi = 0;
    this.refresh = function (e) {
        ////var value1 = $('textarea[name="querytext"]').val();
        ////var value2 = $('textarea[name="queryparam"]').val();
        $.ajax({
            url: '/Test/GetExplain',
            type: 'POST',
            //data: {
            //    "querytext": value1,
            //    "queryparam": value2},
            success: function (response) {
                this.CreateHtml(response);
                this.CreateResult(response);
                this.CreateExplain(response);
                this.JsonTraverse(JSON.parse(response.explain[0])[0]);
                //$('#output1').text(str);
                var html = [];
                html.push(` <div class = "_row">${str}`);
                $('#JsonD').append(html.join());
            }.bind(this)
        })
    };

    this.CreateExplain = function (response) {
            var jsonp = response.explain[0];
            var Node = '';
            var obj = $.parseJSON(jsonp);
           
        var formattedData = JSON.stringify(obj, null, '\t');
        $('#output').text(formattedData);
    };

    this.JsonTraverse = function (object) {
        for (var keys in object) {
            for (var key in object[keys]) {
                if (key == "Node Type") {
                    //str += '<dt>' + '<img src="~/images/hash.png" />'+ object[keys]["Node Type"] + '</dt>';
                    if (object[keys]["Node Type"] == "Seq Scan") {
                        row += 1;
                        //index += 1;
                        str += '<div class =  "column" id="a' + idi++ + '" >' + '<img src="../images/seq.svg" style="width:50px;height:60x; "/>' + object[keys]["Relation Name"] + '</div></div>';
                        this.draw('a'+idi,'a'+idi--);
                    }
                    if (object[keys]["Node Type"] == "Hash") {
                        
                        str += '<div class = "_row">';
                        for ( i=0; i < index; i++) {
                            str += '<div class =  "column"></div>';
                        }
                        index += 1;
                        str += '<div class =  "column" id="a' + idi++ +'" >' + '<img src="../images/hash.svg" style="width:50px;height:60x; "/>' + object[keys]["Node Type"]  + '</div>';
                    }
                    if (object[keys]["Node Type"] == "Hash Join") {
                        index += 1;
                        str += '<div class =  "column" id="a' + idi++ +'" >' + '<img src="../images/hash.svg" style="width:50px;height:60x; "/>' + object[keys]["Node Type"]  + '</div>';
                    }
                }
                if (key == "Plans") {
                    //str += '</div ><div style="float: right;">';
                    this.JsonTraverse(object[keys][key]);
                }
            }
        }
        
    }

    this.draw = function (start, stop) {
        new LeaderLine(
            document.getElementById(''+start+''),
            document.getElementById(''+stop+'')
        );
    }
    

    this.CreateHtml = function (resp) {
        var html = [];
        html.push(`<table  class="table table-bordered sqltbl explain-tbl ">
        <thead>
        <th>Query</th>
        <th>Explain</th>
        <th>Rowno</th>
    </thead>
    <tbody>
        `);

        html.push(`<tqueryplanr>
            <td>${resp.qstring}</td>
            <td>${this.getFromArray(resp.explain)}</td>
            <td>${resp.rowno}</td>
            <tr>`);
        $('#item').append(html.join());
    };


    this.CreateResult = function (resp) {
        var html = [];
        html.push(`<table class="table table-bordered sqltbl result-tbl">
                        <thead>
                            ${this.getThead(resp.resultobj.type)}
                        </thead>
                        <tbody>
                            ${this.getTbody(resp.resultobj.result)}
                        </tbody>
                </table>`);

        $('#item').append(html.join(""));
    };

    this.getThead = function (typeArray) {
        let arry = [];
        for (var i = 0; i < typeArray.length; i++)
        {
            arry.push(` <th>${typeArray[i]}</th>`)
        }
        return arry.join('');
    }

    this.getTbody = function (typeList) { // [[a,b,c],[ka,kb,kc]]
        let array = [];
        for (var i = 0; i < typeList.length; i++) {
            array.push(`<tr>`);
            for (var j = 0; j < typeList[i].length; j++) {
                array.push(`<td>${typeList[i][j]}</td>`)
            }
            array.push(`<tr>`);
        }
        return array.join('');
    }

    this.getFromArray = function (ar) {
        let h = new Array();
        for (i = 0; i < ar.length; i++) {
            h.push(`<div class="expl_byline" style="width:100%;">${ar[i]}</div>`)
        }
        return h.join("");
    }

    this.start = function () {
        $("#explaine_btn").off("click").on("click", this.refresh.bind(this));
    };

    this.start();
}            
    