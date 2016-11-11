function foo(str) {
    info.innerHTML = str || "foo";
    alert('xxxx')
    //window.external.Bar('xx')
}

function initData(data) {
    if (!data) return null;

    if (typeof data == "string")
        data = JSON.parse(data);
    if (data.length === 1) {
        return {
            count: [100.00],
            data :data
        }
    }

    var max = Math.max.apply(null, data);
    var min = Math.min.apply(null, data);
    var ava = (max - min) / 9;


    var ret = {
        count: [],
        data: []
    }

    for (var i = 0; i < 10; i++) {
        var d = min + i * ava;
        ret.data.push(d.toFixed(2));
        var percent = (count(data, function (each) {
            return each >= d - ava / 2 && each < d + ava / 2;
        }) * 100 /data.length).toFixed(2)
        ret.count.push(percent)
    }

    return ret;
}

function showChart(data) {
    var xdata = initData(data);
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: xdata.data,
            datasets: [{
                label: '百分比',
                data: xdata.count,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                pointHoverRadius: 5,
                borderDashOffset: 0.0,
                borderWidth: 1
            }]
        },
        options: {
            scaleLabel: "<%=value%>aaa"
        }
    });
}

function count(data, iterator) {
    var ret = 0;

    for (var i = 0; i < data.length; i++) {
        if (iterator(data[i])) {
            ret++;
        }
    }

    return ret;
}

