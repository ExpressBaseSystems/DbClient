var SqlEval = function () {
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
            }.bind(this)
        })
    };

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
    