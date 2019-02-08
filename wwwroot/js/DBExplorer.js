let Eb_DBExplorer = function (options) {

    this.TCobj = options.TCobj;

    this.create_tree = function (e) {
        let $e = $(e.target);
        $e.children('div').toggle();
        $e.filter('.parent').toggleClass('expanded');
        return false;
    };

    this.ajax_call = function (e) {
        e.preventDefault();

        var data = editor.getValue();

        $.ajax({
            type: "POST",
            url: "https://localhost:44304/Connect/SqlQuery",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: { sql: JSON.stringify(data) },
            traditional: true,
            success: function (result) {
                alert('Yay! It worked!');
                this.ajax_reply();
            }.bind(this),
            error: function (result) {
                alert('Oh no :(  : ' + result);
            }
        });
    }.bind(this);

    this.ajax_reply = function (e) {
        var SqlEval = function () {
            this.refresh = function (e) {
                $.ajax({
                    url: '/Connect/SqlQuery',
                    type: 'GET',
                    success: function (response) {
                        this.CreateHtml(response);
                    }.bind(this)
                })
            }.bind(this);

            this.CreateHtml = function (resp) {
                var html = [];
                html.push(`<table id="records_table" style="width:100%" padding: 20px> <thead> <th>Query</th> <th>Explain</th> <th>Rowno</th> </thead> <tbody>`);

                html.push(`<tr>
                <td>${resp.qstring}</td>
                <td>${this.getFromArray(resp.explain)}</td>
                <td>${resp.rowno}</td>
                <tr>`);
                $('#item').append(html.join());
            };

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
        }.bind(this)            

    }
    this.onDrop = function (evt, ui) {
        let $source = $(ui.draggable);
        let tableName = $source.parent().attr("table-name");
        if (tableName) {
            let posLeft = event.pageX;
            let posTop = event.pageY;
            let $tableBoxHtml = $(`<div is-draggable="false" class="table-box"><i class="fa fa-window-close-o" aria-hidden="true" onClick="parentNode.remove()"></i></div>`);
            $('.cont').append($tableBoxHtml);
            $tableBoxHtml.css("left", posLeft + "px");
            $tableBoxHtml.css("top", posTop + "px");
            $($tableBoxHtml).append(tableName, "<br />");
            let cname = this.TCobj[tableName].Columns[0]['ColumnName'];
            $.each(this.TCobj[tableName].Columns, function (key, column) {
                $($tableBoxHtml).append(column['ColumnName']);
                $($tableBoxHtml).append(" : ", column['ColumnType'], "<br />");
            })
            if ($tableBoxHtml.attr("is-draggable") == "false") {// if called first time
                $tableBoxHtml.draggable(options);
                $tableBoxHtml.attr("is-draggable", "true");
            }
        }  
    }.bind(this);

    window.onload = function () {
        var mime = 'text/x-mariadb';
        // get mime type
        if (window.location.href.indexOf('mime=') > -1) {
            mime = window.location.href.substr(window.location.href.indexOf('mime=') + 5);
        }
        window.editor = CodeMirror.fromTextArea(document.getElementById('code'), {
            mode: mime,
            indentWithTabs: true,
            smartIndent: true,
            lineNumbers: true,
            matchBrackets: true,
            autofocus: true,
            cursorActive: true,
            extraKeys: { "Ctrl-Space": "autocomplete" },
            hintOptions: {
                tables: {
                    users: ["name", "score", "birthDate"],
                    countries: ["name", "population", "size"]
                }
            }
        });
    };

    this.makeDrop = function () {
        $("#droppable").droppable({ drop: this.onDrop });
    };

    this.makeDraggable = function () {
        let options = new Object();
        options = {
            appendTo: 'body', // Append to the body.
            helper: 'clone',
            containment: $('document'),
            revert: 'invalid'
        };
        $('.table-name').draggable(options);
  
    };

    this.pannelhide = function () {
        $('#DRAG').click(function () {
            $('#droppable').toggle();
        });
        
    }

    this.init = function () {
        $('.mytree div:has(div)').addClass('parent');
        $('div.mytree div').click(this.create_tree);
        $('#sqlquery').click(this.ajax_call);
        this.makeDraggable();
        this.makeDrop();
        this.pannelhide();
        $('#QUERY').click(function () {
            $('div.CodeMirror.cm-s-default').toggle();
        });
    };

    this.init();
}

