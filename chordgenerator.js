/**
* Setup the string, load the inital chord, setup
* example chords
*/
function load() {
    var strings = ['E', 'A', 'D', 'G', 'B', 'e'];
    for (var i in strings) {
        window['f' + strings[i]] = $('f' + strings[i]);
        window['p' + strings[i]] = $('p' + strings[i]);
    }
    window.positions = [pE, pA, pD, pG, pB, pe];
    window.fingers = [fE, fA, fD, fG, fB, fe];

    var examples = $('examples');
    for (var i in examples.childNodes) {
        var node = examples.childNodes[i];
        if (node.tagName && node.tagName.toUpperCase() == 'A') {
            node.onclick = function () {
                parseChord(this.href.split('#')[1]);
            }
        }
    }
    url = document.location.href;
    if (url.indexOf('#') != -1) {
        parseChord(url.substr(url.indexOf('#') + 1));
    } else {
        parseChord('D.png?p=xx0232&f=---132');
    }
}

/**
* Returns the html element with the given id
*/
function $(id) {
    return document.getElementById(id);
}

/**
* Parse a chordname of the form name.png?p=positions&f=fingers&s=size
* and display it in the textboxes and an image.
*/
function parseChord(chord) {
    var parts = chord.split('?');
    $('chordname').value = unescapeName(parts[0]).replace(/\.png$/, '');

    var p = (chord.match(/p(os)?=([^&]+)/) || {})[2];
    var f = (chord.match(/f(ingers)?=([^&]+)/) || {})[2];
    var s = (chord.match(/s(ize)?=([^&]+)/) || {})[2];
    var e = (chord.match(/e(xpanded)?=([^&]+)/) || {})[2];

    var doubleDigit = true;
    var tempParts;

    if (p) {
        if (p.indexOf('-') === -1) {
            doubleDigit = false;
        }
        else {
            var tempParts = p.split("-");
            if (tempParts.length < 6) {
                doubleDigit = false;
            }
        }
    }

    for (var i = 0; i < 6; i++) {
        if (f) {
            fingers[i].value = f.charAt(i);
        }
        if (p) {
            if (!doubleDigit) {
                positions[i].value = p.charAt(i);
            }
            else {
                positions[i].value = tempParts[i];
            }
        }
    }

    $('size').value = s || '2';

    if (e && e == 'true') {
        $('expanded').checked = true
    } else {
        $('expanded').checked = false
    }

    showChord();
}

function escapeName(name) {
    return name.replace(/#/g, '%23').replace(/\//, '%2f');
}

function unescapeName(name) {
    return name.replace(/%23/, '#').replace(/%2f/gi, '/');
}

/**
* Shows a chord image based on the values in the textboxes.
*/
function showChord() {

    for (var i in positions) {
        var p = positions[i];
        if (!p.value.match(/^(1|2)?\d|x$/i)) {
            alert('Fret position must be a number from 0-24 or X!');
            p.focus();
            return;
        }
    }

    for (var i in fingers) {
        var f = fingers[i];
        if (!f.value.match(/^1|2|3|4|T|-$/i)) {
            alert('Fingerings must be one of 1,2,3,4,T or -.');
            f.focus();
            return;
        }
    }

    var name = $('chordname').value;
    var chord = pE.value + '-' + pA.value + '-' + pD.value + '-' + pG.value + '-' + pB.value + '-' + pe.value;
    var size = $('size').value;
    var expanded = $('expanded').checked
    if (chord.length == 11) {
        chord = chord.replace(/-/g, '');
    }
    name = escapeName(name);
    var fingers = fE.value + fA.value + fD.value + fG.value + fB.value + fe.value;
    var chordUrl = name + '.png?p=' + chord + '&f=' + fingers + '&s=' + size + '&e=' + expanded;
    var url = 'http://' + document.location.host + document.location.pathname.replace('index.html', '');
    url += chordUrl;

    if (window.analyticsId) {
        ga('send', 'pageview', {
            'page': '/' + chordUrl,
            'title': name + ': ' + chord
        });
    }

    document.location = '#' + chordUrl;

    $('chord-link').setAttribute('href', url);
    $('chord-link').innerHTML = url;
    $('chord-image-link').setAttribute('href', url);

    var image = $('chord-image');
    image.setAttribute('src', url);
    image.setAttribute('alt', unescapeName(name) + ' chord');
    image.setAttribute('title', unescapeName(name) + ' chord');
}