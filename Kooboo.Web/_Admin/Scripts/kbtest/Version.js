
function Version_Contructor_init()
{
    var version = new Kooboo.Version("0.0.2"); 
    expect(version._major).to.be(0);
    expect(version._minor).to.be(0);
    expect(version._patch).to.be(2);
    expect(version.toString()).to.be('0.0.2');
     
}

function Version_IsNewThan()
{
    var versionone = new Kooboo.Version("3.4.2");
    var versiontwo = new Kooboo.Version("2.6.5"); 
    expect(versionone.isNewerThan(versiontwo)).to.be(true);

}

function Version_IsValid() {
    var version1 = new Kooboo.Version("-1.4.2");
    var version2 = new Kooboo.Version("-2.43.2");
    var version3 = new Kooboo.Version("0.0.0");
    var version4 = new Kooboo.Version("3.4.65");

    expect(version1.isValid()).to.be(false);
    expect(version2.isValid()).to.be(false);
    expect(version3.isValid()).to.be(false);
    expect(version4.isValid()).to.be(true);

}