﻿; (function () {
    window.loadDisqus = function () {
        var disqus_shortname = '{{DISQUS_SHORTNAME_MARKER}}';

        var dsq = document.createElement('script');
        dsq.type = 'text/javascript';
        dsq.async = true;
        dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';

        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    };

    window.onload = function () {
        document.body.style.display = 'none';
        document.body.style.paddingBottom = '0';
        document.documentElement.style.height = "100%";
        document.documentElement.style.backgroundPosition = "50% 50%";
        document.documentElement.style.backgroundRepeat = "no-repeat";
        document.documentElement.style.backgroundImage = "url(data:image/gif;base64,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)";

        var mdBody = document.body.innerHTML;
        var xmpTag = '<xmp theme="' + document.scripts[0].getAttribute('theme') + '">' + mdBody + '</xmp><hr /><div id="disqus_thread" class="container"><a class="btn btn-large btn-primary" onclick="loadDisqus();">Need comment?</a></div><hr />';
        document.body.innerHTML = xmpTag;

        var strapdown = document.createElement('script');
        strapdown.type = 'text/javascript';
        strapdown.src = 'http://strapdownjs.com/v/0.2/strapdown.js';

        document.body.appendChild(strapdown);
    };
})();