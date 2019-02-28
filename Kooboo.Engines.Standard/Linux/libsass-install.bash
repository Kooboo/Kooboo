#https://github.com/sass/sassc/blob/master/docs/building/unix-instructions.md

#Clone the SassC repo
git clone https://github.com/sass/sassc.git

#Run CI bootstrap script and import env-variables
. sassc/script/bootstrap

#Execute make to compile all sources
make -C sassc -j4

#install the resulting binary
PREFIX="/usr" make -C sassc install