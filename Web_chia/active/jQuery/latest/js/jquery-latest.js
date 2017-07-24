$(function () {
    // 先取得 div#abgne_marquee ul
    // 接著把 ul 中的 li 項目再重覆加入 ul 中(等於有兩組內容)
    // 再來取得 div#abgne_marquee 的高來決定每次跑馬燈移動的距離
    // 設定跑馬燈移動的速度及輪播的速度
    var $marqueeUl = $('div#abgne_marquee ul'),
			$marqueeli = $marqueeUl.append($marqueeUl.html()).children(),
			_height = $('div#abgne_marquee').height() * -1,
			scrollSpeed = 600,
			timer,
			speed = 3000 + scrollSpeed,
			direction = 0, // 0 表示往上, 1 表示往下
			_lock = false;

    // 先把 $marqueeli 移動到第二組
    $marqueeUl.css('top', $marqueeli.length / 2 * _height);

    // 幫左邊 $marqueeli 加上 hover 事件
    // 當滑鼠移入時停止計時器；反之則啟動
    $marqueeli.hover(function () {
        clearTimeout(timer);
    }, function () {
        timer = setTimeout(showad, speed);
    });

    // 判斷要往上還是往下
    $('div#abgne_marquee .marquee_btn').click(function () {
        if (_lock) return;
        clearTimeout(timer);
        direction = $(this).attr('id') == 'marquee_next_btn' ? 0 : 1;
        showad();
    });

    // 控制跑馬燈上下移動的處理函式
    function showad() {
        _lock = !_lock;
        var _now = $marqueeUl.position().top / _height;
        _now = (direction ? _now - 1 + $marqueeli.length : _now + 1) % $marqueeli.length;

        // $marqueeUl 移動
        $marqueeUl.animate({
            top: _now * _height
        }, scrollSpeed, function () {
            // 如果已經移動到第二組時...則馬上把 top 設回到第一組的最後一筆
            // 藉此產生不間斷的輪播
            if (_now == $marqueeli.length - 1) {
                $marqueeUl.css('top', $marqueeli.length / 2 * _height - _height);
            } else if (_now == 0) {
                $marqueeUl.css('top', $marqueeli.length / 2 * _height);
            }
            _lock = !_lock;
        });

        // 再啟動計時器
        timer = setTimeout(showad, speed);
    }

    // 啟動計時器
    timer = setTimeout(showad, speed);

    $('a').focus(function () {
        this.blur();
    });
});
