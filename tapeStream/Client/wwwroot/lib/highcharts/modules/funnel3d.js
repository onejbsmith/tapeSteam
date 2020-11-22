/*
 Highcharts JS v8.2.2 (2020-10-22)

 Highcharts funnel module

 (c) 2010-2019 Kacper Madej

 License: www.highcharts.com/license
*/
(function(c){"object"===typeof module&&module.exports?(c["default"]=c,module.exports=c):"function"===typeof define&&define.amd?define("highcharts/modules/funnel3d",["highcharts","highcharts/highcharts-3d","highcharts/modules/cylinder"],function(g){c(g);c.Highcharts=g;return c}):c("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(c){function g(c,m,g,x){c.hasOwnProperty(m)||(c[m]=x.apply(null,g))}c=c?c._modules:{};g(c,"Series/Funnel3DSeries.js",[c["Core/Color/Color.js"],c["Core/Globals.js"],
c["Extensions/Math3D.js"],c["Core/Series/Series.js"],c["Core/Utilities.js"]],function(c,m,g,x,l){var t=c.parse,H=m.charts;c=m.Renderer.prototype;var J=g.perspective,K=x.seriesTypes,L=l.error,E=l.extend,p=l.merge,C=l.pick,A=l.relativeLength,M=c.cuboidPath;x.seriesType("funnel3d","column",{center:["50%","50%"],width:"90%",neckWidth:"30%",height:"110%",neckHeight:"25%",reversed:!1,gradientForSides:!0,animation:!1,edgeWidth:0,colorByPoint:!0,showInLegend:!1,dataLabels:{align:"right",crop:!1,inside:!1,
overflow:"allow"}},{bindAxes:function(){m.Series.prototype.bindAxes.apply(this,arguments);E(this.xAxis.options,{gridLineWidth:0,lineWidth:0,title:null,tickPositions:[]});E(this.yAxis.options,{gridLineWidth:0,title:null,labels:{enabled:!1}})},translate3dShapes:m.noop,translate:function(){m.Series.prototype.translate.apply(this,arguments);var a=0,b=this.chart,d=this.options,h=d.reversed,k=d.ignoreHiddenPoint,e=b.plotWidth,c=b.plotHeight,f=0,I=d.center,v=A(I[0],e),u=A(I[1],c),p=A(d.width,e),q,g,n=A(d.height,
c),l=A(d.neckWidth,e),t=A(d.neckHeight,c),B=u-n/2+n-t;e=this.data;var y,x,w,z,G,D,r;this.getWidthAt=g=function(b){var a=u-n/2;return b>B||n===t?l:l+(p-l)*(1-(b-a)/(n-t))};this.center=[v,u,n];this.centerX=v;e.forEach(function(b){k&&!1===b.visible||(a+=b.y)});e.forEach(function(e){G=null;y=a?e.y/a:0;w=u-n/2+f*n;z=w+y*n;q=g(w);D=z-w;r={gradientForSides:C(e.options.gradientForSides,d.gradientForSides),x:v,y:w,height:D,width:q,z:1,top:{width:q}};q=g(z);r.bottom={fraction:y,width:q};w>=B?r.isCylinder=!0:
z>B&&(G=z,q=g(B),z=B,r.bottom.width=q,r.middle={fraction:D?(B-w)/D:0,width:q});h&&(r.y=w=u+n/2-(f+y)*n,r.middle&&(r.middle.fraction=1-(D?r.middle.fraction:0)),q=r.width,r.width=r.bottom.width,r.bottom.width=q);e.shapeArgs=E(e.shapeArgs,r);e.percentage=110*y;e.plotX=v;e.plotY=h?u+n/2-(f+y/2)*n:(w+(G||z))/2;x=J([{x:v,y:e.plotY,z:h?-(p-g(e.plotY))/2:-g(e.plotY)/2}],b,!0)[0];e.tooltipPos=[x.x,x.y];e.dlBoxRaw={x:v,width:g(e.plotY),y:w,bottom:r.height,fullWidth:p};k&&!1===e.visible||(f+=y)})},alignDataLabel:function(a,
b,d){var h=a.dlBoxRaw,c=this.chart.inverted,e=a.plotY>C(this.translatedThreshold,this.yAxis.len),F=C(d.inside,!!this.options.stacking),f={x:h.x,y:h.y,height:0};d.align=C(d.align,!c||F?"center":e?"right":"left");d.verticalAlign=C(d.verticalAlign,c||F?"middle":e?"top":"bottom");"top"!==d.verticalAlign&&(f.y+=h.bottom/("bottom"===d.verticalAlign?1:2));f.width=this.getWidthAt(f.y);this.options.reversed&&(f.width=h.fullWidth-f.width);F?f.x-=f.width/2:"left"===d.align?(d.align="right",f.x-=1.5*f.width):
"right"===d.align?(d.align="left",f.x+=f.width/2):f.x-=f.width/2;a.dlBox=f;K.column.prototype.alignDataLabel.apply(this,arguments)}},{shapeType:"funnel3d",hasNewShapeType:m.seriesTypes.column.prototype.pointClass.prototype.hasNewShapeType});g=p(c.elements3d.cuboid,{parts:"top bottom frontUpper backUpper frontLower backLower rightUpper rightLower".split(" "),mainParts:["top","bottom"],sideGroups:["upperGroup","lowerGroup"],sideParts:{upperGroup:["frontUpper","backUpper","rightUpper"],lowerGroup:["frontLower",
"backLower","rightLower"]},pathType:"funnel3d",opacitySetter:function(a){var b=this,d=b.parts,c=m.charts[b.renderer.chartIndex],k="group-opacity-"+a+"-"+c.index;b.parts=b.mainParts;b.singleSetterForParts("opacity",a);b.parts=d;c.renderer.filterId||(c.renderer.definition({tagName:"filter",id:k,children:[{tagName:"feComponentTransfer",children:[{tagName:"feFuncA",type:"table",tableValues:"0 "+a}]}]}),b.sideGroups.forEach(function(a){b[a].attr({filter:"url(#"+k+")"})}),b.renderer.styledMode&&(c.renderer.definition({tagName:"style",
textContent:".highcharts-"+k+" {filter:url(#"+k+")}"}),b.sideGroups.forEach(function(b){b.addClass("highcharts-"+k)})));return b},fillSetter:function(a){var b=this,d=t(a),c=d.rgba[3],k={top:t(a).brighten(.1).get(),bottom:t(a).brighten(-.2).get()};1>c?(d.rgba[3]=1,d=d.get("rgb"),b.attr({opacity:c})):d=a;d.linearGradient||d.radialGradient||!b.gradientForSides||(d={linearGradient:{x1:0,x2:1,y1:1,y2:1},stops:[[0,t(a).brighten(-.2).get()],[.5,a],[1,t(a).brighten(-.2).get()]]});d.linearGradient?b.sideGroups.forEach(function(a){var c=
b[a].gradientBox,e=d.linearGradient,h=p(d,{linearGradient:{x1:c.x+e.x1*c.width,y1:c.y+e.y1*c.height,x2:c.x+e.x2*c.width,y2:c.y+e.y2*c.height}});b.sideParts[a].forEach(function(b){k[b]=h})}):(p(!0,k,{frontUpper:d,backUpper:d,rightUpper:d,frontLower:d,backLower:d,rightLower:d}),d.radialGradient&&b.sideGroups.forEach(function(a){var d=b[a].gradientBox,c=d.x+d.width/2,e=d.y+d.height/2,h=Math.min(d.width,d.height);b.sideParts[a].forEach(function(a){b[a].setRadialReference([c,e,h])})}));b.singleSetterForParts("fill",
null,k);b.color=b.fill=a;d.linearGradient&&[b.frontLower,b.frontUpper].forEach(function(a){(a=(a=a.element)&&b.renderer.gradients[a.gradient])&&"userSpaceOnUse"!==a.attr("gradientUnits")&&a.attr({gradientUnits:"userSpaceOnUse"})});return b},adjustForGradient:function(){var a=this,b;a.sideGroups.forEach(function(d){var c={x:Number.MAX_VALUE,y:Number.MAX_VALUE},k={x:-Number.MAX_VALUE,y:-Number.MAX_VALUE};a.sideParts[d].forEach(function(d){b=a[d].getBBox(!0);c={x:Math.min(c.x,b.x),y:Math.min(c.y,b.y)};
k={x:Math.max(k.x,b.x+b.width),y:Math.max(k.y,b.y+b.height)}});a[d].gradientBox={x:c.x,width:k.x-c.x,y:c.y,height:k.y-c.y}})},zIndexSetter:function(){this.finishedOnAdd&&this.adjustForGradient();return this.renderer.Element.prototype.zIndexSetter.apply(this,arguments)},onAdd:function(){this.adjustForGradient();this.finishedOnAdd=!0}});c.elements3d.funnel3d=g;c.funnel3d=function(a){var b=this.element3d("funnel3d",a),d=this.styledMode,c={"stroke-width":1,stroke:"none"};b.upperGroup=this.g("funnel3d-upper-group").attr({zIndex:b.frontUpper.zIndex}).add(b);
[b.frontUpper,b.backUpper,b.rightUpper].forEach(function(a){d||a.attr(c);a.add(b.upperGroup)});b.lowerGroup=this.g("funnel3d-lower-group").attr({zIndex:b.frontLower.zIndex}).add(b);[b.frontLower,b.backLower,b.rightLower].forEach(function(a){d||a.attr(c);a.add(b.lowerGroup)});b.gradientForSides=a.gradientForSides;return b};c.funnel3dPath=function(a){this.getCylinderEnd||L("A required Highcharts module is missing: cylinder.js",!0,H[this.chartIndex]);var b=H[this.chartIndex],c=a.alphaCorrection=90-Math.abs(b.options.chart.options3d.alpha%
180-90),h=M.call(this,p(a,{depth:a.width,width:(a.width+a.bottom.width)/2})),k=h.isTop,e=!h.isFront,g=!!a.middle,f=this.getCylinderEnd(b,p(a,{x:a.x-a.width/2,z:a.z-a.width/2,alphaCorrection:c})),m=a.bottom.width,v=p(a,{width:m,x:a.x-m/2,z:a.z-m/2,alphaCorrection:c}),u=this.getCylinderEnd(b,v,!0),l=m,q=v,t=u,n=u;g&&(l=a.middle.width,q=p(a,{y:a.y+a.middle.fraction*a.height,width:l,x:a.x-l/2,z:a.z-l/2}),t=this.getCylinderEnd(b,q,!1),n=this.getCylinderEnd(b,q,!1));h={top:f,bottom:u,frontUpper:this.getCylinderFront(f,
t),zIndexes:{group:h.zIndexes.group,top:0!==k?0:3,bottom:1!==k?0:3,frontUpper:e?2:1,backUpper:e?1:2,rightUpper:e?2:1}};h.backUpper=this.getCylinderBack(f,t);f=1!==Math.min(l,a.width)/Math.max(l,a.width);h.rightUpper=this.getCylinderFront(this.getCylinderEnd(b,p(a,{x:a.x-a.width/2,z:a.z-a.width/2,alphaCorrection:f?-c:0}),!1),this.getCylinderEnd(b,p(q,{alphaCorrection:f?-c:0}),!g));g&&(f=1!==Math.min(l,m)/Math.max(l,m),p(!0,h,{frontLower:this.getCylinderFront(n,u),backLower:this.getCylinderBack(n,u),
rightLower:this.getCylinderFront(this.getCylinderEnd(b,p(v,{alphaCorrection:f?-c:0}),!0),this.getCylinderEnd(b,p(q,{alphaCorrection:f?-c:0}),!1)),zIndexes:{frontLower:e?2:1,backLower:e?1:2,rightLower:e?1:2}}));return h}});g(c,"masters/modules/funnel3d.src.js",[],function(){})});
//# sourceMappingURL=funnel3d.js.map