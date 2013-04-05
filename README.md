Fantomas
========

An F# source code formatter, similar to [scalariform](https://github.com/mdr/scalariform) for Scala, [ocp-indent](https://github.com/OCamlPro/ocp-indent) for OCaml and [PythonTidy](https://github.com/acdha/PythonTidy) for Python.

## Purpose
This project aims at formatting F# source files based on a given configuration.
Fantomas will ensure correct indentation and consistent spacing between elements in the source files.
We assume that the source files are *parsable by F# compiler* before feeding into the tool.
Fantomas follows the formatting guideline being described in [A comprehensive guide to F# Formatting Conventions](FormattingConventions.md).

## Use cases
The project is developed with the following use cases in mind:
 - Reformatting an unfamiliar code base. It gives readability when you are not the one originally written the code. 
To illustrate, the following example

	```fsharp
	type Type
	    = TyLam of Type * Type
	    | TyVar of string
	    | TyCon of string * Type list
	    with override this.ToString () =
	            match this with
	            | TyLam (t1, t2) -> sprintf "(%s -> %s)" (t1.ToString()) (t2.ToString())
	            | TyVar a -> a
	            | TyCon (s, ts) -> s
	 ```
will be rewritten to

	```fsharp
	type Type = 
	  | TyLam of Type * Type
	  | TyVar of string
	  | TyCon of string * Type list
	  override this.ToString() = 
	    match this with
	    | TyLam(t1, t2) -> sprintf "(%s -> %s)" (t1.ToString()) (t2.ToString())
	    | TyVar a -> a
	    | TyCon(s, ts) -> s
	 ```

 - Converting from verbose syntax to light syntax. 
Feeding a source file in verbose mode, Fantomas will format it appropriately in light mode.
This might be helpful for code generation since generating verbose source files is much easier.
For example, this code fragment

	```fsharp
	#light "off"
	let Multiple9x9 () = 
	  for i in 1 .. 9 do
	    printf "\n";
	    for j in 1 .. 9 do
	      let k = i * j in
	      printf "%d x %d = %2d " i j k;
	      done;
	  done;;
	Multiple9x9 ();;
	```	
is reformulated to 
	```fsharp
	let Multiple9x9() = 
	  for i in 1..9 do
	    printf "\n"
	    for j in 1..9 do
	      let k = i * j
	      printf "%d x %d = %2d " i j k
	
	Multiple9x9()
	```

For more complex examples, please take a look at F# outputs of [20 language shootout programs](tests/languageshootout_output) and [10 CodeReview.SE source files](tests/stackexchange_output).

## How to use
Fantomas can be used as a command line tool or an API.
You can fork this repo and compile the project with F# 3.0/.NET framework 4.0. 
Alternatively, I have also created a [NuGet package](https://nuget.org/packages/Fantomas/).
For detailed guidelines, please read [Fantomas: How to use](Usage.md).

## Installation
The code base is written in F# 3.0/.NET framework 4.0. 
The solution file can be opened in Visual Studio 2012 and MonoDevelop or Xamarin Studio.
NuGet is used to manage external packages.
The [test project](src/Fantomas.Tests) depends on FsUnit and NUNit.
However, the [library project](src/Fantomas) and [command line interface](src/Fantomas.Cmd) have no dependency on external packages.

## Testing and validation
I have tried to be careful in testing the project.
There are 84 unit tests and 30 validated test examples, 
but it seems many corner cases of the language haven't been covered.
Feel free to suggests tests if they haven't been handled correctly.

## Limitations
Due to limited information in F# ASTs, beware of the following drawbacks:
 - Inline comments and multiline comments are lost. Only XML doc comments are preserved.
 - Compiler directives are lost.
 - Multiple attributes are displayed separately.
 - Properties with both get and set are displayed separately. 

## Why the name "Fantomas"?
There are a few reason to choose the name as such. 
First, it starts with an "F", just like many other F# projects. 
Second, Fantomas is my favourite character in the literature. 
Finally, Fantomas means "ghost" in French; coincidentally F# ASTs and formatting rules are so *mysterious* to be handled correctly.

## Acknowledgments
I would like to gratefully thank [EricGT](https://github.com/EricGT) for his encouragement and feedbacks.
Without his help, this project will not complete, let alone be useful.

## License
The library and tool are available under Apache 2.0. 
For more information see the [License file](LICENSE.md).
