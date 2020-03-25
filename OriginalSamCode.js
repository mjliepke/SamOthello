var e=2.718281828459045;
var size=[64,32,16,8,4,1];
var weights=[null];
var outs=[];

var delta=[null];
var r=function(){
  return(0.5-Math.random())};
  
for(var i=1;i<size.length;i++){
  weights[i]=[];
  
  for(var j=0;j<size[i];j++){
    weights[i][j]=[];
    for(var k=0;k<size[i-1]+1;k++){weights[i][j][k]=r()}}}
	
for(var i=0;i<size.length;i++){
  outs[i]=[];
  if(i!=0){delta[i]=[]}
  for(var j=0;j<size[i];j++){
    outs[i][j]=0;
    if(i!=0){delta[i][j]=0}}}
	
var sigmoid=function(x){
  return(1/(1+Math.pow(e,-x)))};
  
var derivative=function(x){
  return(x*(1-x))};
  
var forward=function(inp,answer){
  outs[0]=inp;
  
  for(var l=1;l<weights.length;l++){
    var inps=[];
    for(var pn=0;pn<outs[l-1].length;pn++){appendItem(inps,outs[l-1][pn])}
    for(var n=0;n<weights[l].length;n++){
      var sum=0;
      for(var w=0;w<inps.length+1;w++){
        if(w==inps.length){sum+=weights[l][n][w]}
        else{sum+=weights[l][n][w]*outs[l-1][w]}}
      outs[l][n]=sigmoid(sum)}}
	  
  if(answer==true){return(outs[outs.length-1])}};
  
var back=function(expected){
  for(var i=0;i<outs[outs.length-1].length;i++){
    delta[outs.length-1][i]=(expected[i]-outs[outs.length-1][i])*derivative(outs[outs.length-1][i]);
  }
  for(var l=outs.length-2;l>0;l--){
    for(var n=0;n<outs[l].length;n++){
      var sum=0;
      for(var fn=0;fn<weights[l+1].length;fn++){
        sum+=weights[l+1][fn][n]*delta[l+1][fn];
      }
      delta[l][n]=sum*derivative(outs[l][n]);
    }
  }
  for(var wl=1;wl<weights.length;wl++){
    for(var wn=0;wn<weights[wl].length;wn++){
      for(var ww=0;ww<weights[wl][wn].length;ww++){
        if(ww==weights[wl][wn].length-1){
          weights[wl][wn][ww]+=0.5*delta[wl][wn];
        } else{
          weights[wl][wn][ww]+=0.5*delta[wl][wn]*outs[wl-1][ww]}}}}
};

var round=function(num){
  return (Math.round(10000*num)/10000);
};

var train=function(inp,goal,display){
  forward(inp); back([goal]);
  var table=[];
  for(var i=0;i<outs[outs.length-1].length;i++){
    table[i]=round(outs[outs.length-1][i]);
  }
  if(display==true){console.log("expected: "+goal+", output: "+table)}
};

// --------- //

var pieces=[];
var bplays=[];
var wplays=[];
var player=0;
var nomoves=false;
var bscore=2;
var wscore=2;

setActiveCanvas("canvas1");
setStrokeWidth(2);
setStrokeColor(rgb(50,50,50));

var changecor=true;
var decisions=[];
var winner=0;

var setbox=function(x,y,cor){
  if(cor==1){
    setFillColor(rgb(120,240,250));
  }else if(cor==0){setFillColor(rgb(180,10,10))}
  else if(cor==-1){setFillColor(rgb(170,120,60))}
  circle(40*x+20,40*y+20,18);
};

var countscore=function(){
  bscore=0; wscore=0;
  for (var i=0;i<8;i++){
    for(var j=0;j<8;j++){
      if (pieces[i][j]==0){bscore++}
      else if(pieces[i][j]==1){wscore++}
    }
  }
};

var resetboard=function(){
  for (var k=0;k<8;k++){
    pieces[k]=[null,null,null,null,null,null,null,null,null];
    bplays[k]=[0,0,0,0,0,0,0,0];
    wplays[k]=[0,0,0,0,0,0,0,0];
  }
  if(changecor==true){
    for(var i=0;i<8;i++){for(var j=0;j<8;j++){
      setbox(i,j,-1);
    }}
  }
  pieces[3][3]=1;
  pieces[4][4]=1;
  pieces[4][3]=0;
  pieces[3][4]=0;
  setbox(3,3,1);
  setbox(4,4,1);
  setbox(4,3,0);
  setbox(3,4,0);
  calcplays(0);
};

var resetcalc=function(cor){
  var table=[];
  if(cor==1){table=wplays}else{table=bplays}
  for (var k=0;k<8;k++){
    table[k]=[0,0,0,0,0,0,0,0];
  }
};

var calcplays=function(cor){
  var table;
  nomoves=true;
  if(cor==1){table=wplays}else{table=bplays}
  resetcalc(cor);
  for(var i=0;i<8;i++){
    for(var j=0;j<8;j++){
      if(pieces[i][j]==cor){
        check(i,j,cor);
  }}}
};

var check=function(x,y,cor){
  for(var i=-1;i<2;i++){
    for(var j=-1;j<2;j++){
      if(pieces[x+i]!=undefined){
      if(pieces[x+i][y+j]==1-cor){
        checkdirection(x,y,cor,i,j);
  }}}}
};

var checkdirection=function(x,y,cor,xx,yy){
  var table;
  if(cor==0){table=bplays}else{table=wplays}
  var notdone=true;
  var ax=x; var ay=y;
  while(notdone){
    ax+=xx; ay+=yy;
    if(ax>7||ax<0||ay>7||ay<0||pieces[ax][ay]==cor||(pieces[ax][ay]!=0&&pieces[ax][ay]!=1&&pieces[ax][ay]!=null)){
      notdone=false;
    } else if(pieces[ax][ay]==null){table[ax][ay]=1; nomoves=false; notdone=false}
  }
};

var linechange=function(x,y,gx,gy,cor,tablei){
  var dx=0;
  if (gx>x){dx=1}else if(gx<x){dx=-1}
  var dy=0;
  if (gy>y){dy=1}else if(gy<y){dy=-1}
  var count=0;
  while(!(gx==x&&gy==y)&&count<10){
    count++;
    tablei[gx][gy]=cor;
    if(tablei==pieces&&changecor==true){
      setbox(gx,gy,cor);
    }
    gx-=dx; gy-=dy;
  }
};

var checkplay=function(x,y,cor,xx,yy,tablej,noreset){
  var table;
  if(cor==0){table=bplays}else{table=wplays}
  var notdone=true;
  var ax=x; var ay=y;
  while(notdone){
    ax+=xx; ay+=yy;
    if(ax>7||ax<0||ay>7||ay<0||pieces[ax][ay]==null||(pieces[ax][ay]!=0&&pieces[ax][ay]!=1&&pieces[ax][ay]!=null)){
      notdone=false;
    } else if(pieces[ax][ay]==cor){if(noreset!=true){resetcalc(cor)} linechange(ax,ay,x,y,cor,tablej); notdone=false}
  }
};

var testtable=[];
for(var aahh=0;aahh<8;aahh++){testtable[aahh]=[]; for(var bbhh=0;bbhh<8;bbhh++){
  testtable[aahh][bbhh]=null;
}}
var calcmove=function(x,y){
  copytable(pieces,testtable,true);
  for(var i=-1;i<2;i++){
    for(var j=-1;j<2;j++){
      if(pieces[x+i]!=undefined){
      if(pieces[x+i][y+j]==1-player){
        checkplay(x,y,player,i,j,testtable,true);
  }}}}
  return testtable;
};

var play=function(x,y){
  for(var i=-1;i<2;i++){
    for(var j=-1;j<2;j++){
      if(pieces[x+i]!=undefined){
      if(pieces[x+i][y+j]==1-player){
        checkplay(x,y,player,i,j,pieces);
  }}}}
  player=1-player;
  calcplays(player);
  if(nomoves==true){
    player=1-player;
    calcplays(player);
    if(nomoves==true){
      countscore();
      if(wscore>bscore){winner=1}
      else if(bscore>wscore){winner=0}
      else{winner=-1}
    }else{makeplay(player)}
  } else{makeplay(player)}
};

var copytable=function(table0,table1,d2){
  for(var t=0;t<table0.length;t++){
    if(d2==true){table1[t]=[];
      for(var u=0;u<table0[t].length;u++){
        table1[t][u]=table0[t][u];
      }
    }else{table1[t]=table0[t]}
  }
  return table1;
};

var maxarray=function(table,givenum){
  var max=-Infinity;
  var num=0;
  for(var i=0;i<table.length;i++){
    if(table[i]>max){max=table[i]; num=i}
  }
  if(givenum==true){return num}
  return max;
};

var makeplay=function(plr){
  appendItem(decisions,[]);
  var table;
  if(plr==1){table=wplays}else{table=bplays}
  var prob=[];
  var num=-1;
  var currentstates=[];
  for(var i=0;i<8;i++){for(var j=0;j<8;j++){
    if(table[i][j]==1){
      num++;
      appendItem(prob,[null,i,j]);
      var fstate2=calcmove(i,j);
      var fstate=[];
      for(var k=0;k<8;k++){for(var l=0;l<8;l++){
        var numer=8*l+k;
        fstate[numer]=fstate2[k][l];
        if (fstate[numer]==null){fstate[numer]=2}
        else if(fstate[numer]==plr){fstate[numer]=0}
        else if(fstate[numer]==1-plr){fstate[numer]=1}
      }}
      var expecta=forward(fstate,true)[0];
      prob[num][0]=expecta;
      appendItem(currentstates,fstate);
    }
  }}
  var array0=[];
  for(var w=0;w<prob.length;w++){
    appendItem(array0,prob[w][0]);
  }
  var max=maxarray(array0);
  var nummax=maxarray(array0,true);
  var choice=prob[nummax];
  decisions[decisions.length-1][0]=currentstates[nummax];
  decisions[decisions.length-1][1]=max;
  play(choice[1],choice[2]);
};

var domatch=function(){
  decisions=[];
  resetboard();
  player=0;
  makeplay(0);
  console.log(winner+" won");
  var draw=false;
  if(winner==-1){draw=true}
  for(var playnum=0;playnum<decisions.length;playnum++){
    if((playnum+winner)%2==0&&draw===false){
      train(decisions[playnum][0],1);
    }else{
      train(decisions[playnum][0],0);
    }
  }
};

var logweights=function(){
  var string="[null";
  for(var i=1;i<weights.length;i++){
    string+=",[";
    for(var j=0;j<weights[i].length;j++){
      if(j!=0){string+=","}
      string+="[";
      string+=weights[i][j];
      string+="]";
    }
    string+="]";
  }
  string+="]";
  console.log(string);
};

var sstopp=false;
var counter=0;

for(var doit=0;doit<Infinity;doit++){
  if(sstopp!=true){domatch()}
  counter++;
}