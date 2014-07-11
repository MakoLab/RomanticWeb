# GitHub page for [RomanticWeb](http://github.org/MakoLab/RomanticWeb) project

## About

The site is generated into static HTML pages using [Jekyll](http://jekyllrb.com).

## Running locally

To run the generation on dev machine it is required to install the components required by Jekyll. 

Console must be run as admin.

### Ruby 2 and Ruby DevKit

1. Install Ruby 2.0 with [Chocolatey](http://chocolatey.org)

    cinst ruby

2. Download the matching Ruby DevKit downloaded from [Ruby installer page](http://rubyinstaller.org/downloads/). 
3. Unpack it to c:\devkit
4. **For 64bit** edit the file dk.rb and replace a similar part with

    REG_KEYS = [
      'Software\RubyInstaller\MRI',
      'Software\RubyInstaller\Rubinius',
      'Software\Wow6432Node\RubyInstaller\MRI'
    ]
	
5. Open the console 

    cd c:\devkit
    ruby dk.rb init
    ruby dk.rb install
	
### Install Jekyll gem

    gem install jekyll --all
    gem install jekyll --version 1.4.2
    gem uninstall pygments.rb --all
    gem install pygments.rb --version 0.5.0
	
### Install pygments with [Chocolatey](http://chocolatey.org)

    cinst python2
    cinst easy.install
    cinst pip
    pip install pygments
	
### **OPTIONAL** install WDM to enable file watch

    gem install wdm
	
### Run jekyll

    jekyll serve --watch
    
### Preview the pages locally

    Browse to http://localhost:4000/RomanticWeb/
