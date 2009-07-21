function $(id) {
    return document.getElementById(id);
}

function parseChord(chord) {
    var parts = chord.split('/');
    $('chordname').value = parts[0];
                
    for (var i = 0; i < 6; i++) {
        fingers[i].value = parts[2].charAt(i);
        positions[i].value = parts[1].charAt(i);
    }
    showChord();
}

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
    var chord = pE.value+'-'+pA.value+'-'+pD.value+'-'+pG.value+'-'+pB.value+'-'+pe.value;
    var size = $('size').value;
    if (chord.length == 11) {
        chord = chord.replace(/-/g, '');
    }
    var fingers = fE.value+fA.value+fD.value+fG.value+fB.value+fe.value;
    var chordUrl = [name,chord,fingers,size].join('/');
    var url = 'http://' + document.location.host + '/' + chordUrl;
    document.location = '#' + chordUrl;
    
    $('chord-link').setAttribute('href', url);
    $('chord-link').innerHTML = url;
    
    $('chord-image-link').setAttribute('href', url);

    var image = $('chord-image');
    image.setAttribute('src', url);
    image.setAttribute('alt', name + ' chord');
    image.setAttribute('title', name + ' chord');
}

 function load(){
    var strings = ['E', 'A', 'D', 'G', 'B', 'e'];    
    for (var i in strings) {
        window['f'+strings[i]] = $('f'+strings[i]);
        window['p'+strings[i]] = $('p'+strings[i]);
    }
    window.positions = [pE, pA, pD, pG, pB, pe];        
    window.fingers = [fE, fA, fD, fG, fB, fe];        
    var examples = $('examples');
    for (var i in examples.childNodes) {
        var node = examples.childNodes[i];
        if (node.tagName && node.tagName.toUpperCase() == 'A') {
            node.onclick = function() {
                parseChord(this.href.split('#')[1]);
            }
        }
    }
    parseChord(location.fragment || 'D/xx0232/---132');
 }
