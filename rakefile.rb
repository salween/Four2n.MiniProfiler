require 'rubygems'
require 'bundler/setup'

gem 'albacore'
require 'albacore'
require 'rake/clean'

ORCHARD_VERSION = "1.6"

dirs = {
    :build          => File.expand_path("build"),
    :orchard        => File.expand_path("src/Orchard").gsub("/", "\\"),
    :dependencies   => File.expand_path("build/dependencies"),
    :libs           => File.expand_path("lib"),
    :src            => File.expand_path("src"),
    :tools          => File.expand_path("tools"),
    :buildtools     => File.expand_path("tools/build")
}

CLEAN.clear()
CLEAN.include(dirs[:build] + '/*')
CLEAN.exclude('**/.gitignore')

unzip :extractOrchardDependency do |unzip|
    unzip.destination = dirs[:dependencies]
    unzip.file = File.join(dirs[:libs] + "/Orchard/Orchard.Web.#{ORCHARD_VERSION}.zip")
end

task :copyOrchardDependency => [:extractOrchardDependency] do
    sh "xcopy #{dirs[:dependencies].gsub("/", "\\")}\\Orchard\\*.* #{dirs[:orchard].gsub("/", "\\")}\\ /Y /E /Q /EXCLUDE:#{File.join(dirs[:buildtools] + "/CopyDependencies.excluded").gsub("/", "\\")}"
end

task :copyOrchardLocalizationDependency do
    sh "xcopy #{dirs[:libs].gsub("/", "\\")}\\OrchardLocalizations\\*.* #{dirs[:orchard].gsub("/", "\\")}\\ /Y /E /Q /EXCLUDE:#{File.join(dirs[:buildtools] + "/CopyDependencies.excluded").gsub("/", "\\")}"
end

task :linkOrchardModules do
    modules = FileList.new(dirs[:src] + "/Four2n.MiniProfiler/")
    modules.each do |item|
        link = dirs[:orchard] + "\\Modules\\" + item.split("/").last
        target = item.gsub("/", "\\");
        if Dir.exist?(link) == false
            sh "cmd /C mklink /D #{link} #{target}"
        end
    end
end

task :linkOrchardThemes do
    themes = FileList.new(dirs[:src] + "/app/*OrchardTheme*")
    themes.each do |item|
        link = dirs[:orchard] + "\\Themes\\" + item.split("/").last
        target = item.gsub("/", "\\");
        if Dir.exist?(link) == false
            sh "cmd /C mklink /D #{link} #{target}"
        end
    end
end

desc "Preparing development environment"
task :prepareDevelopment => [:clean, :copyOrchardDependency, :linkOrchardModules, :linkOrchardThemes]

desc "Starts Orchard application through IISExpress"
exec :runIISExpress do |cmd|
    cmd.command = "iisexpress"
    cmd.parameters = "/path:#{dirs[:orchard]}", "/port:888"

end 

