/*
 * Chord Image Generator
 * http://tech.einaregilsson.com/2009/07/23/chord-image-generator/
 *
 * Copyright (C) 2009 Einar Egilsson [einar@einaregilsson.com]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 * $HeadURL$
 * $LastChangedDate$
 * $Author$
 * $Revision$
 */

/**
 * Setup the string, load the inital chord, setup
 * example chords
 */
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
    url = document.location.href;
    if (url.indexOf('#') != -1) {
        parseChord(url.substr(url.indexOf('#')+1));
    } else {
        parseChord('D/xx0232/---132');
    }
}

/**
 * Returns the html element with the given id
 */
function $(id) {
    return document.getElementById(id);
}

/**
 * Parse a chordname of the form /name/positions/fingers/size
 * and display it in the textboxes and an image.
 */
function parseChord(chord) {
    var parts = chord.split('/');
    $('chordname').value = parts[0].replace(/%23/g, '#');
                
    for (var i = 0; i < 6; i++) {
        fingers[i].value = parts[2].charAt(i);
        positions[i].value = parts[1].charAt(i);
    }
    showChord();
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
    var chord = pE.value+'-'+pA.value+'-'+pD.value+'-'+pG.value+'-'+pB.value+'-'+pe.value;
    var size = $('size').value;
    if (chord.length == 11) {
        chord = chord.replace(/-/g, '');
    }
    name = name.replace(/#/g, '%23');
    var fingers = fE.value+fA.value+fD.value+fG.value+fB.value+fe.value;
    var chordUrl = [name,chord,fingers,size].join('/');
    var url = 'http://' + document.location.host + '/';
    url += chordUrl;

    document.location= '#'+chordUrl;   
    
    $('chord-link').setAttribute('href', url);
    $('chord-link').innerHTML = url;
    $('chord-image-link').setAttribute('href', url);

    var image = $('chord-image');
    image.setAttribute('src', url);
    image.setAttribute('alt', name.replace(/%23/g, '#') + ' chord');
    image.setAttribute('title', name.replace(/%23/g, '#') + ' chord');
}