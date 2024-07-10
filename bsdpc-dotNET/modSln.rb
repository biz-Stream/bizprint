#!/usr/bin/ruby

require 'rubygems'
require 'kconv'
require 'fileutils'
require 'optparse'
require 'rexml/document'
require 'rexml/streamlistener'


def okdir( path )
  return false if (nil == path)
  return false if !(File.exist?(path))
  return false if !(File.directory?(path))
  return true
end

def okfile( path )
  return false if (nil == path)
  return false if !(File.exist?(path))
  return false if !(File.file?(path))
  return true
end

def getFileLines( path )
  fullPath = File.expand_path( path )
  lns = Array.new()
  File.open( fullPath, "r" ) {|f|
    lnss = f.readlines
    lnss.each {|ln|
      lns.push( ln.toutf8 )
    }
  }
  return lns
end



opt = OptionParser.new
OPTS = {}
opt.on('-i PRJDIR', 'VisualStudio-Project Directory') {|v| OPTS[:i] = v }
opt.parse!(ARGV)

#====

def getXmlDoc( fpath )
  sxml = ""
  File.open( fpath ) {|f|
    sxml = f.read
  }
  doc = REXML::Document.new(sxml)
  return doc
end


#=== for pom.xml
def getPomXmlVersion( path )
  fullPath = File.expand_path( path )
  xmlDoc = getXmlDoc( fullPath )
  return xmlDoc.elements['project/version'].get_text.value
end
def setPomXmlVersion( path, ver )
  fullPath = File.expand_path( path )
  dots = v2array( ver, 3 )
  version = dots[0] + "." + dots[1] + "." + dots[2]
  xmlDoc = getXmlDoc( fullPath )
  xmlDoc.elements['project/version'].text.value = version
  File.open( fullPath, "w+" ) {|f|
    xmlDoc.write(f)
  }
  return true
end

#=== for AssemblyInfo.cs
def getAsmInfCsVersion( path )
  fullPath = File.expand_path( path )
  #[assembly: AssemblyVersion("2.17.1201")]
  lns = getFileLines(fullPath)
  retVer = ""
  lns.each {|ln|
    next if !(/^\[assembly.*AssemblyVersion(.*)\]/ === ln)
    ver = $&
    vers = ver.split("\"")
    next if !(3 == vers.size)
    retVer = vers[1]
    break
  }
  return retVer
end
def setAsmInfCsVersion( path, ver )
  fullPath = File.expand_path( path )
  #[assembly: AssemblyVersion("2.12.7.30")]
  lns = getFileLines(fullPath)
  dots = v2array(ver, 4)
  version = dots[0] + "." + dots[1] + "." + dots[2]# + "." + dots[3]
  tmpLns = Array.new(lns)
  lns.each_index {|idx|
    next if !(/^\[assembly.*AssemblyVersion(.*)\]/ === lns[idx])
    tmpLns[idx] = "[assembly: AssemblyVersion(\"" + version + "\")]"
    break
  }
  File.open( fullPath, "w" ) {|f|
    tmpLns.each {|ln|
      f.puts(ln.tosjis)
    }
  }
end

#====
def callVsCmdBld( slnPath )
  return if (nil == slnPath)
  path = File.expand_path(slnPath)
  return if !(okfile(path))
  puts "Execute: Msbuild.exe  /p:Configuration=Release /t:Rebuild \"#{path}\""
  ret = `Msbuild.exe  /p:Configuration=Release /t:Rebuild "#{path}"`
  puts ret
end
def v2array( ver, num )
  return nil if !( 3 == num || 4 == num)
  dots = ver.split(".")
  dots = ver.split(",") if (3 > dots.size)
  return nil if (3 > dots.size)
  return dots if ( num == dots.size )
  (dots[2] = dots[2] + dots[3]; dots.delete_at(3);) if ( 3 == num )
  if (4 == num)
    if ( 3 == dots[2].length )
      dots.push( dots[2][1,2] )
      dots[2] = dots[2][0,1]
    else
      dots.push( dots[2][2,2])
      dots[2] = dots[2][0,2]
    end
  end
  return dots
end

#==== for debug
#puts getPomXmlVersion(ARGV[0])
#setPomXmlVersion(ARGV[0], ARGV[1])
#puts getAsmInfCsVersion(ARGV[0])
#setAsmInfCsVersion(ARGV[0], ARGV[1])


InfoFileArray = ["src/Properties/AssemblyInfo.cs"]


def mkIns( srcDir )
  puts srcDir
  srcDir = srcDir + "/" if !( "/" == srcDir[srcDir.size - 1])
  verSion = getPomXmlVersion( srcDir + "pom.xml")
  (puts opt.help; exit 0;) if (nil == verSion || verSion.empty?)

    InfoFileArray.each {|info|
      setAsmInfCsVersion( srcDir + info , verSion )
    }


  callVsCmdBld( srcDir + "src/DirectPrintClientModule.sln")
end

#==========

sDir = OPTS[:i]

sDir = "" if (nil == sDir)

sDir = "./" if (sDir.empty?)

sDir = File.expand_path(sDir)

(puts opt.help; exit 0;) if !(okdir(sDir))

mkIns( sDir )

